import { createFileRoute, Link } from '@tanstack/react-router';
import { dayService } from '../../../api';

export const Route = createFileRoute('/administration/day/')({
    loader: async () => dayService.getAll(),
    component: DayListPage,
});

function formatDate(value: string) {
    return new Date(value).toLocaleDateString();
}

function DayListPage() {
    const days = Route.useLoaderData();

    return (
        <div className="stack">
            <section className="card actions-row">
                <h2>Dani takmičenja</h2>
                <Link to="/administration/day/new" className="button-link">
                    Novi dan
                </Link>
            </section>

            <section className="card">
                {days.length === 0 ? (
                    <p>Nema unetih dana.</p>
                ) : (
                    <table className="table">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Datum</th>
                                <th>Osnovna cena</th>
                                <th>Akcije</th>
                            </tr>
                        </thead>
                        <tbody>
                            {days.map((day) => (
                                <tr key={day.id}>
                                    <td>{day.id}</td>
                                    <td>{formatDate(day.date)}</td>
                                    <td>{day.basePrice}</td>
                                    <td className="actions-inline">
                                        <Link to="/administration/day/$id" params={{ id: String(day.id) }}>
                                            Detalji
                                        </Link>
                                        <Link to="/administration/day/$id/edit" params={{ id: String(day.id) }}>
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