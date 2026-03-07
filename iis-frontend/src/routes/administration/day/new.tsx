import { createFileRoute, useNavigate } from '@tanstack/react-router';
import { useState } from 'react';
import { dayService } from '../../../api';

export const Route = createFileRoute('/administration/day/new')({
    component: NewDayPage,
});

function NewDayPage() {
    const navigate = useNavigate();

    const [date, setDate] = useState('');
    const [basePrice, setBasePrice] = useState('');
    const [error, setError] = useState('');
    const [submitting, setSubmitting] = useState(false);

    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        setError('');

        if (!date || !basePrice) {
            setError('Sva polja su obavezna.');
            return;
        }

        setSubmitting(true);

        try {
            await dayService.create({
                date,
                basePrice: Number(basePrice),
            });

            navigate({ to: '/administration/day' });
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Došlo je do greške.');
        } finally {
            setSubmitting(false);
        }
    }

    return (
        <section className="card">
            <h2>Novi dan</h2>

            {error && <div className="alert error">{error}</div>}

            <form onSubmit={handleSubmit} className="form">
                <label>
                    Datum
                    <input type="date" value={date} onChange={(e) => setDate(e.target.value)} />
                </label>

                <label>
                    Osnovna cena
                    <input
                        type="number"
                        step="0.01"
                        value={basePrice}
                        onChange={(e) => setBasePrice(e.target.value)}
                    />
                </label>

                <button type="submit" disabled={submitting}>
                    {submitting ? 'Čuvanje...' : 'Sačuvaj'}
                </button>
            </form>
        </section>
    );
}