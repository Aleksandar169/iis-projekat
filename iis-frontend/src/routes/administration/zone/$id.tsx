import { createFileRoute, Link } from '@tanstack/react-router';
import { zoneService } from '../../../api';

export const Route = createFileRoute('/administration/zone/$id')({
    loader: async ({ params }) => zoneService.getById(Number(params.id)),
    component: ZoneDetailsPage,
});

function ZoneDetailsPage() {
    const zone = Route.useLoaderData();

    return (
        <section className="card">
            <h2>Detalji zone</h2>

            <p>
                <strong>ID:</strong> {zone.id}
            </p>
            <p>
                <strong>Karakteristike:</strong> {zone.characteristics}
            </p>
            <p>
                <strong>Kapacitet:</strong> {zone.capacity}
            </p>
            <p>
                <strong>Dodatak na cenu:</strong> {zone.priceAddon}
            </p>

            <div className="actions-row">
                <Link to="/administration/zone/$id/edit" params={{ id: String(zone.id) }} className="button-link">
                    Izmeni
                </Link>
                <Link to="/administration/zone" className="button-link secondary">
                    Nazad
                </Link>
            </div>
        </section>
    );
}