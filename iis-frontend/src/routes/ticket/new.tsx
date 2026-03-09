import { createFileRoute, Link } from '@tanstack/react-router';
import { useEffect, useState } from 'react';
import { competitionService, ticketService } from '../../api';
import type { CalculateTicketResponseDto, TicketSelectionDto } from '../../model/ticket';

export const Route = createFileRoute('/ticket/new')({
    loader: async () => competitionService.getCompetition(),
    component: NewTicketPage,
});

function formatDate(value: string) {
    return new Date(value).toLocaleDateString();
}

function NewTicketPage() {
    const competition = Route.useLoaderData();

    const getDefaultSelections = (): TicketSelectionDto[] =>
        competition.days.length > 0 && competition.zones.length > 0
            ? [{ dayId: competition.days[0].id, zoneId: competition.zones[0].id }]
            : [];

    const getDefaultCurrencyId = () =>
        competition.allowedCurrencies[0]?.id ? String(competition.allowedCurrencies[0].id) : '';

    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [addressLine, setAddressLine] = useState('');
    const [postalCode, setPostalCode] = useState('');
    const [city, setCity] = useState('');
    const [country, setCountry] = useState('');
    const [email, setEmail] = useState('');
    const [confirmEmail, setConfirmEmail] = useState('');
    const [promoCode, setPromoCode] = useState('');
    const [selectedCurrencyId, setSelectedCurrencyId] = useState(getDefaultCurrencyId());

    const [selections, setSelections] = useState<TicketSelectionDto[]>(getDefaultSelections());

    const [error, setError] = useState('');
    const [submitting, setSubmitting] = useState(false);

    const [pricePreview, setPricePreview] = useState<CalculateTicketResponseDto | null>(null);
    const [priceError, setPriceError] = useState('');
    const [priceLoading, setPriceLoading] = useState(false);

    const [purchaseResult, setPurchaseResult] = useState<{ email: string; ticketCode: string } | null>(null);
    const [isSuccessModalOpen, setIsSuccessModalOpen] = useState(false);

    function resetForm() {
        setFirstName('');
        setLastName('');
        setAddressLine('');
        setPostalCode('');
        setCity('');
        setCountry('');
        setEmail('');
        setConfirmEmail('');
        setPromoCode('');
        setSelectedCurrencyId(getDefaultCurrencyId());
        setSelections(getDefaultSelections());

        setError('');
        setPriceError('');
        setPricePreview(null);
    }

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

    useEffect(() => {
        async function loadPreview() {
            setPriceError('');
            setPricePreview(null);

            if (!selectedCurrencyId || selections.length === 0) return;

            const dayIds = selections.map((s) => s.dayId);
            if (new Set(dayIds).size !== dayIds.length) {
                setPriceError('Za isti dan ne možeš izabrati više zona u jednoj karti.');
                return;
            }

            setPriceLoading(true);

            try {
                const preview = await ticketService.calculate({
                    selections,
                    promoCode: promoCode.trim() ? promoCode.trim() : null,
                    selectedCurrencyId: Number(selectedCurrencyId),
                });

                setPricePreview(preview);
            } catch (err) {
                setPriceError(err instanceof Error ? err.message : 'Greška pri obračunu cene.');
            } finally {
                setPriceLoading(false);
            }
        }

        loadPreview();
    }, [selections, promoCode, selectedCurrencyId]);

    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        setError('');

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

            setPurchaseResult(response);
            setIsSuccessModalOpen(true);
            resetForm();
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Došlo je do greške.');
        } finally {
            setSubmitting(false);
        }
    }

    return (
        <>
            <div className="stack">
                <section className="card">
                    <h2>Kupovina karte</h2>

                </section>

                {error && <div className="alert error">{error}</div>}

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
                            Valuta plaćanja
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
                                                    {formatDate(day.date)}
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
                                                    {zone.characteristics}
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

                        <section className="card" style={{ marginTop: '8px' }}>
                            <h3>Pregled cijene</h3>

                            {priceLoading && <p>Obračun cijene...</p>}
                            {priceError && <div className="alert error">{priceError}</div>}

                            {pricePreview && (
                                <>
                                    <table className="table">
                                        <thead>
                                            <tr>
                                                <th>Dan</th>
                                                <th>Zona</th>
                                                <th>Osnovna cijena dana (RSD)</th>
                                                <th>Dodatak zone (RSD)</th>
                                                <th>Ukupno stavka (RSD)</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {pricePreview.items.map((item) => (
                                                <tr key={`${item.dayId}-${item.zoneId}`}>
                                                    <td>{formatDate(item.dayDate)}</td>
                                                    <td>{item.zoneCharacteristics}</td>
                                                    <td>{item.basePriceRsd.toFixed(2)}</td>
                                                    <td>{item.zoneAddonRsd.toFixed(2)}</td>
                                                    <td>{item.itemTotalRsd.toFixed(2)}</td>
                                                </tr>
                                            ))}
                                        </tbody>
                                    </table>

                                    <div className="stack" style={{ marginTop: '12px' }}>
                                        <p>
                                            <strong>Međuzbir (RSD):</strong> {pricePreview.subtotalRsd.toFixed(2)}
                                        </p>

                                        <p>
                                            <strong>Popust 10% (RSD):</strong>{' '}
                                            {pricePreview.dateDiscountApplied
                                                ? `- ${pricePreview.dateDiscountAmountRsd.toFixed(2)}`
                                                : 'Ne važi'}
                                        </p>

                                        <p>
                                            <strong>Promo popust 5% (RSD):</strong>{' '}
                                            {pricePreview.promoDiscountApplied
                                                ? `- ${pricePreview.promoDiscountAmountRsd.toFixed(2)}`
                                                : 'Nije primenjen'}
                                        </p>

                                        <p>
                                            <strong>Ukupno nakon popusta (RSD):</strong>{' '}
                                            {pricePreview.totalAfterDiscountsRsd.toFixed(2)}
                                        </p>

                                        <p>
                                            <strong>Kurs:</strong> {pricePreview.exchangeRate.toFixed(4)}
                                        </p>

                                        <p>
                                            <strong>Ukupno za plaćanje ({pricePreview.currencyCode}):</strong>{' '}
                                            {pricePreview.finalAmount.toFixed(2)}
                                        </p>
                                    </div>
                                </>
                            )}
                        </section>

                        <button type="submit" disabled={submitting || !!priceError || !pricePreview}>
                            {submitting ? 'Slanje...' : 'Pošalji zahtev za kupovinu'}
                        </button>
                    </form>
                </section>
            </div>

            {isSuccessModalOpen && purchaseResult && (
                <div
                    style={{
                        position: 'fixed',
                        inset: 0,
                        background: 'rgba(0,0,0,0.45)',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        padding: '16px',
                        zIndex: 1000,
                    }}
                >
                    <div
                        style={{
                            background: 'white',
                            borderRadius: '8px',
                            padding: '24px',
                            width: '100%',
                            maxWidth: '520px',
                            border: '1px solid #d1d5db',
                        }}
                    >
                        <h3>Uspešno poslat zahtev za kupovinu</h3>

                        <p>
                            <strong>Email:</strong> {purchaseResult.email}
                        </p>

                        <p>
                            <strong>Šifra karte:</strong> {purchaseResult.ticketCode}
                        </p>

                        <p>
                            Sačuvaj email i šifru. Na stranici za pregled karte možeš kasnije proveriti
                            status obrade zahteva.
                        </p>

                        <div className="actions-row">
                            <button type="button" onClick={() => setIsSuccessModalOpen(false)}>
                                Zatvori
                            </button>

                            <Link to="/ticket/view" className="button-link">
                                Idi na pregled karte
                            </Link>
                        </div>
                    </div>
                </div>
            )}
        </>
    );
}