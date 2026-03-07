import { createFileRoute, useNavigate } from '@tanstack/react-router';
import { useState } from 'react';
import { zoneService } from '../../../api';

export const Route = createFileRoute('/administration/zone/$id_/edit')({
    loader: async ({ params }) => zoneService.getById(Number(params.id)),
    component: EditZonePage,
});

function EditZonePage() {
    const navigate = useNavigate();
    const zone = Route.useLoaderData();

    const [characteristics, setCharacteristics] = useState(zone.characteristics);
    const [capacity, setCapacity] = useState(String(zone.capacity));
    const [priceAddon, setPriceAddon] = useState(String(zone.priceAddon));
    const [error, setError] = useState('');
    const [submitting, setSubmitting] = useState(false);
    const [deleting, setDeleting] = useState(false);

    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        setError('');

        if (!characteristics || !capacity || !priceAddon) {
            setError('Sva polja su obavezna.');
            return;
        }

        setSubmitting(true);

        try {
            await zoneService.update(zone.id, {
                characteristics,
                capacity: Number(capacity),
                priceAddon: Number(priceAddon),
            });

            navigate({ to: '/administration/zone' });
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Došlo je do greške.');
        } finally {
            setSubmitting(false);
        }
    }

    async function handleDelete() {
        setError('');
        const confirmed = window.confirm('Da li sigurno želiš da obrišeš ovu zonu?');
        if (!confirmed) return;

        setDeleting(true);

        try {
            await zoneService.delete(zone.id);
            navigate({ to: '/administration/zone' });
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Došlo je do greške.');
        } finally {
            setDeleting(false);
        }
    }

    return (
        <section className="card">
            <h2>Izmena zone</h2>

            {error && <div className="alert error">{error}</div>}

            <form onSubmit={handleSubmit} className="form">
                <label>
                    Karakteristike
                    <input
                        value={characteristics}
                        onChange={(e) => setCharacteristics(e.target.value)}
                    />
                </label>

                <label>
                    Kapacitet
                    <input
                        type="number"
                        value={capacity}
                        onChange={(e) => setCapacity(e.target.value)}
                    />
                </label>

                <label>
                    Dodatak na cenu
                    <input
                        type="number"
                        step="0.01"
                        value={priceAddon}
                        onChange={(e) => setPriceAddon(e.target.value)}
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