import { createFileRoute, useNavigate } from '@tanstack/react-router';
import { useState } from 'react';
import { zoneService } from '../../../api';

export const Route = createFileRoute('/administration/zone/new')({
    component: NewZonePage,
});

function NewZonePage() {
    const navigate = useNavigate();

    const [characteristics, setCharacteristics] = useState('');
    const [capacity, setCapacity] = useState('');
    const [priceAddon, setPriceAddon] = useState('');
    const [error, setError] = useState('');
    const [submitting, setSubmitting] = useState(false);

    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        setError('');

        if (!characteristics || !capacity || !priceAddon) {
            setError('Sva polja su obavezna.');
            return;
        }

        setSubmitting(true);

        try {
            await zoneService.create({
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

    return (
        <section className="card">
            <h2>Nova zona</h2>

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

                <button type="submit" disabled={submitting}>
                    {submitting ? 'Čuvanje...' : 'Sačuvaj'}
                </button>
            </form>
        </section>
    );
}