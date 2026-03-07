import { createFileRoute, Link, useNavigate } from '@tanstack/react-router';
import { useMemo, useState } from 'react';
import { competitionService, currencyService } from '../../api';

export const Route = createFileRoute('/administration/')({
    loader: async () => {
        const [competition, currencies] = await Promise.all([
            competitionService.tryGetCompetition(),
            currencyService.getAll(),
        ]);

        return { competition, currencies };
    },
    component: AdministrationPage,
});

function toDateInputValue(value?: string | null) {
    if (!value) return '';
    return new Date(value).toISOString().slice(0, 10);
}

function AdministrationPage() {
    const navigate = useNavigate();
    const { competition, currencies } = Route.useLoaderData();

    const initialSelectedCurrencies = useMemo(
        () => competition?.allowedCurrencies.map((c) => c.id) ?? [],
        [competition],
    );

    const [name, setName] = useState(competition?.name ?? '');
    const [location, setLocation] = useState(competition?.location ?? '');
    const [startDate, setStartDate] = useState(toDateInputValue(competition?.startDate));
    const [endDate, setEndDate] = useState(toDateInputValue(competition?.endDate));
    const [discountValidUntil, setDiscountValidUntil] = useState(
        toDateInputValue(competition?.discountValidUntil),
    );
    const [additionalInfo, setAdditionalInfo] = useState(competition?.additionalInfo ?? '');
    const [selectedCurrencyIds, setSelectedCurrencyIds] = useState<number[]>(initialSelectedCurrencies);

    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');
    const [submitting, setSubmitting] = useState(false);

    function toggleCurrency(id: number) {
        setSelectedCurrencyIds((prev) =>
            prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id],
        );
    }

    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        setError('');
        setSuccess('');

        if (!name || !location || !startDate || !endDate || !discountValidUntil) {
            setError('Sva osnovna polja su obavezna.');
            return;
        }

        if (selectedCurrencyIds.length === 0) {
            setError('Moraš izabrati bar jednu dozvoljenu valutu.');
            return;
        }

        setSubmitting(true);

        try {
            await competitionService.updateCompetition({
                name,
                location,
                startDate,
                endDate,
                discountValidUntil,
                additionalInfo,
            });

            await competitionService.setAllowedCurrencies({
                currencyIds: selectedCurrencyIds,
            });

            setSuccess('Osnovne informacije su uspešno sačuvane.');
            navigate({ to: '/administration' });
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Došlo je do greške.');
        } finally {
            setSubmitting(false);
        }
    }

    return (
        <div className="stack">
            <section className="card">
                <h2>Administracija</h2>
                <p>Ovde menjaš osnovne informacije o takmičenju i dozvoljene valute.</p>
            </section>

            <section className="card">
                <h3>Osnovne informacije o takmičenju</h3>

                {error && <div className="alert error">{error}</div>}
                {success && <div className="alert success">{success}</div>}

                <form onSubmit={handleSubmit} className="form">
                    <label>
                        Naziv
                        <input value={name} onChange={(e) => setName(e.target.value)} />
                    </label>

                    <label>
                        Lokacija
                        <input value={location} onChange={(e) => setLocation(e.target.value)} />
                    </label>

                    <label>
                        Datum početka
                        <input
                            type="date"
                            value={startDate}
                            onChange={(e) => setStartDate(e.target.value)}
                        />
                    </label>

                    <label>
                        Datum kraja
                        <input type="date" value={endDate} onChange={(e) => setEndDate(e.target.value)} />
                    </label>

                    <label>
                        Popust 10% važi do
                        <input
                            type="date"
                            value={discountValidUntil}
                            onChange={(e) => setDiscountValidUntil(e.target.value)}
                        />
                    </label>

                    <label>
                        Dodatne informacije
                        <textarea
                            value={additionalInfo}
                            onChange={(e) => setAdditionalInfo(e.target.value)}
                            rows={4}
                        />
                    </label>

                    <fieldset className="fieldset">
                        <legend>Dozvoljene valute</legend>

                        {currencies.length === 0 ? (
                            <p>Nema valuta u bazi.</p>
                        ) : (
                            currencies.map((currency) => (
                                <label key={currency.id} className="checkbox-row">
                                    <input
                                        type="checkbox"
                                        checked={selectedCurrencyIds.includes(currency.id)}
                                        onChange={() => toggleCurrency(currency.id)}
                                    />
                                    {currency.currencyName} ({currency.code})
                                </label>
                            ))
                        )}
                    </fieldset>

                    <button type="submit" disabled={submitting}>
                        {submitting ? 'Čuvanje...' : 'Sačuvaj'}
                    </button>
                </form>
            </section>

            <section className="card actions-row">
                <Link to="/administration/day" className="button-link">
                    Upravljanje danima
                </Link>
                <Link to="/administration/zone" className="button-link">
                    Upravljanje zonama
                </Link>
            </section>
        </div>
    );
}