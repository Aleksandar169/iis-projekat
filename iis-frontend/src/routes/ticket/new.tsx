import { createFileRoute, Link } from '@tanstack/react-router';
import { useState } from 'react';
import { competitionService, ticketService } from '../../api';
import type { TicketSelectionDto } from '../../model/ticket';

export const Route = createFileRoute('/ticket/new')({
    loader: async () => competitionService.getCompetition(),
    component: NewTicketPage,
});

function formatDate(value: string) {
    return new Date(value).toLocaleDateString();
}

function NewTicketPage() {
    const competition = Route.useLoaderData();

    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [addressLine, setAddressLine] = useState('');
    const [postalCode, setPostalCode] = useState('');
    const [city, setCity] = useState('');
    const [country, setCountry] = useState('');
    const [email, setEmail] = useState('');
    const [confirmEmail, setConfirmEmail] = useState('');
    const [promoCode, setPromoCode] = useState('');
    const [selectedCurrencyId, setSelectedCurrencyId] = useState(
        competition.allowedCurrencies[0]?.id ? String(competition.allowedCurrencies[0].id) : '',
    );

    const [selections, setSelections] = useState<TicketSelectionDto[]>(
        competition.days.length > 0 && competition.zones.length > 0
            ? [{ dayId: competition.days[0].id, zoneId: competition.zones[0].id }]
            : [],
    );

    const [error, setError] = useState('');
    const [submitting, setSubmitting] = useState(false);
    const [result, setResult] = useState<{ email: string; ticketCode: string } | null>(null);

    function addSelection() {
        if (competition.days.length === 0 || competition.zones.length === 0) return;
        setSelections((prev) => [
            ...prev,
            {
                dayId: competition.days[0].id,
                zoneId: competition.zones[0].id,
            },
        ]);
    }

    function updateSelection(index: number, field: 'dayId' | 'zoneId', value: number) {
        setSelections((prev) =>
            prev.map((item, i) => (i === index ? { ...item, [field]: value } : item)),
        );
    }

    function removeSelection(index: number) {
        setSelections((prev) => prev.filter((_, i) => i !== index));
    }

    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        setError('');
        setResult(null);

        if (
            !firstName ||
            !lastName ||
            !addressLine ||
            !postalCode ||
            !city ||
            !country ||
            !email ||
            !confirmEmail ||
            !selectedCurrencyId
        ) {
            setError('Sva obavezna polja moraju biti popunjena.');
            return;
        }

        if (email !== confirmEmail) {
            setError('Email i potvrda email adrese moraju biti isti.');
            return;
        }

        if (selections.length === 0) {
            setError('Moraš izabrati bar jedan dan i zonu.');
            return;
        }

        const dayIds = selections.map((s) => s.dayId);
        if (new Set(dayIds).size !== dayIds.length) {
            setError('Za isti dan ne možeš izabrati više zona u jednoj karti.');
            return;
        }

        setSubmitting(true);

        try {
            const response = await ticketService.create({
                firstName,
                lastName,
                addressLine,
                postalCode,
                city,
                country,
                email,
                confirmEmail,
                selections,
                promoCode: promoCode.trim() ? promoCode.trim() : null,
                selectedCurrencyId: Number(selectedCurrencyId),
            });

            setResult(response);
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Došlo je do greške.');
        } finally {
            setSubmitting(false);
        }
    }

    return (
        <div className="stack">
            <section className="card">
                <h2>Kupovina karte</h2>
                <p>
                    Po arhitekturi C, zahtev za kreiranje karte prvo ide u queue. Nakon dobijene
                    šifre proveravaš status karte na stranici za pregled.
                </p>
            </section>

            {error && <div className="alert error">{error}</div>}

            {result && (
                <section className="card">
                    <h3>Zahtev za kupovinu je primljen</h3>
                    <p>
                        <strong>Email:</strong> {result.email}
                    </p>
                    <p>
                        <strong>Šifra karte:</strong> {result.ticketCode}
                    </p>
                    <p>
                        Sačuvaj ovu šifru. Zatim idi na pregled karte i proveri da li je obrada uspešna.
                    </p>
                    <Link to="/ticket/view" className="button-link">
                        Idi na pregled karte
                    </Link>
                </section>
            )}

            <section className="card">
                <h3>Osnovne informacije o takmičenju</h3>
                <p>
                    <strong>Naziv:</strong> {competition.name}
                </p>
                <p>
                    <strong>Lokacija:</strong> {competition.location}
                </p>
                <p>
                    <strong>Trajanje:</strong> {formatDate(competition.startDate)} -{' '}
                    {formatDate(competition.endDate)}
                </p>
                <p>
                    <strong>Popust 10% važi do:</strong> {formatDate(competition.discountValidUntil)}
                </p>
            </section>

            <section className="card">
                <form onSubmit={handleSubmit} className="form">
                    <h3>Podaci kupca</h3>

                    <label>
                        Ime
                        <input value={firstName} onChange={(e) => setFirstName(e.target.value)} />
                    </label>

                    <label>
                        Prezime
                        <input value={lastName} onChange={(e) => setLastName(e.target.value)} />
                    </label>

                    <label>
                        Adresa
                        <input value={addressLine} onChange={(e) => setAddressLine(e.target.value)} />
                    </label>

                    <label>
                        Poštanski broj
                        <input value={postalCode} onChange={(e) => setPostalCode(e.target.value)} />
                    </label>

                    <label>
                        Mesto
                        <input value={city} onChange={(e) => setCity(e.target.value)} />
                    </label>

                    <label>
                        Država
                        <input value={country} onChange={(e) => setCountry(e.target.value)} />
                    </label>

                    <label>
                        Email
                        <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} />
                    </label>

                    <label>
                        Potvrda email adrese
                        <input
                            type="email"
                            value={confirmEmail}
                            onChange={(e) => setConfirmEmail(e.target.value)}
                        />
                    </label>

                    <label>
                        Promo kod (opciono)
                        <input value={promoCode} onChange={(e) => setPromoCode(e.target.value)} />
                    </label>

                    <label>
                        Valuta
                        <select
                            value={selectedCurrencyId}
                            onChange={(e) => setSelectedCurrencyId(e.target.value)}
                        >
                            {competition.allowedCurrencies.map((currency) => (
                                <option key={currency.id} value={currency.id}>
                                    {currency.currencyName} ({currency.code})
                                </option>
                            ))}
                        </select>
                    </label>

                    <fieldset className="fieldset">
                        <legend>Odabir dana i zona</legend>

                        {selections.map((selection, index) => (
                            <div key={index} className="selection-row">
                                <label>
                                    Dan
                                    <select
                                        value={selection.dayId}
                                        onChange={(e) => updateSelection(index, 'dayId', Number(e.target.value))}
                                    >
                                        {competition.days.map((day) => (
                                            <option key={day.id} value={day.id}>
                                                {formatDate(day.date)} - osnovna cena {day.basePrice}
                                            </option>
                                        ))}
                                    </select>
                                </label>

                                <label>
                                    Zona
                                    <select
                                        value={selection.zoneId}
                                        onChange={(e) => updateSelection(index, 'zoneId', Number(e.target.value))}
                                    >
                                        {competition.zones.map((zone) => (
                                            <option key={zone.id} value={zone.id}>
                                                {zone.characteristics} - kapacitet {zone.capacity} - dodatak {zone.priceAddon}
                                            </option>
                                        ))}
                                    </select>
                                </label>

                                <button
                                    type="button"
                                    className="danger"
                                    onClick={() => removeSelection(index)}
                                    disabled={selections.length === 1}
                                >
                                    Ukloni red
                                </button>
                            </div>
                        ))}

                        <button type="button" onClick={addSelection}>
                            Dodaj još jedan dan
                        </button>
                    </fieldset>

                    <button type="submit" disabled={submitting}>
                        {submitting ? 'Slanje...' : 'Pošalji zahtev za kupovinu'}
                    </button>
                </form>
            </section>
        </div>
    );
}