import { createFileRoute, useNavigate } from '@tanstack/react-router';
import { useState } from 'react';
import { dayService } from '../../../api';

export const Route = createFileRoute('/administration/day/$id_/edit')({
    loader: async ({ params }) => dayService.getById(Number(params.id)),
    component: EditDayPage,
});

function toDateInputValue(value: string) {
    return new Date(value).toISOString().slice(0, 10);
}

function EditDayPage() {
    const navigate = useNavigate();
    const day = Route.useLoaderData();

    const [date, setDate] = useState(toDateInputValue(day.date));
    const [basePrice, setBasePrice] = useState(String(day.basePrice));
    const [error, setError] = useState('');
    const [submitting, setSubmitting] = useState(false);
    const [deleting, setDeleting] = useState(false);

    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        setError('');

        if (!date || !basePrice) {
            setError('Sva polja su obavezna.');
            return;
        }

        setSubmitting(true);

        try {
            await dayService.update(day.id, {
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

    async function handleDelete() {
        setError('');
        const confirmed = window.confirm('Da li sigurno želiš da obrišeš ovaj dan?');
        if (!confirmed) return;

        setDeleting(true);

        try {
            await dayService.delete(day.id);
            navigate({ to: '/administration/day' });
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Došlo je do greške.');
        } finally {
            setDeleting(false);
        }
    }

    return (
        <section className="card">
            <h2>Izmena dana</h2>

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

                <div className="actions-row">
                    <button type="submit" disabled={submitting}>
                        {submitting ? 'Čuvanje...' : 'Sačuvaj izmene'}
                    </button>

                    <button
                        type="button"
                        className="danger"
                        onClick={handleDelete}
                        disabled={deleting}
                    >
                        {deleting ? 'Brisanje...' : 'Obriši'}
                    </button>
                </div>
            </form>
        </section>
    );
}