import { createFileRoute, Link } from '@tanstack/react-router';
import { zoneService } from '../../../api';

export const Route = createFileRoute('/administration/zone/')({
    loader: async () => zoneService.getAll(),
    component: ZoneListPage,
});

function ZoneListPage() {
    const zones = Route.useLoaderData();

    return (
        <div className="stack">
            <section className="card actions-row">
                <h2>Zone sedenja</h2>
                <Link to="/administration/zone/new" className="button-link">
                    Nova zona
                </Link>
            </section>

            <section className="card">
                {zones.length === 0 ? (
                    <p>Nema unetih zona.</p>
                ) : (
                    <table className="table">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Karakteristike</th>
                                <th>Kapacitet</th>
                                <th>Dodatak na cenu</th>
                                <th>Akcije</th>
                            </tr>
                        </thead>
                        <tbody>
                            {zones.map((zone) => (
                                <tr key={zone.id}>
                                    <td>{zone.id}</td>
                                    <td>{zone.characteristics}</td>
                                    <td>{zone.capacity}</td>
                                    <td>{zone.priceAddon}</td>
                                    <td className="actions-inline">
                                        <Link to="/administration/zone/$id" params={{ id: String(zone.id) }}>
                                            Detalji
                                        </Link>
                                        <Link to="/administration/zone/$id/edit" params={{ id: String(zone.id) }}>
                                            Izmeni
                                        </Link>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                )}
            </section>
        </div>
    );
}