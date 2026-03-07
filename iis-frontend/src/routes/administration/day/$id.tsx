import { createFileRoute, Link } from '@tanstack/react-router';
import { dayService } from '../../../api';

export const Route = createFileRoute('/administration/day/$id')({
    loader: async ({ params }) => dayService.getById(Number(params.id)),
    component: DayDetailsPage,
});

function formatDate(value: string) {
    return new Date(value).toLocaleDateString();
}

function DayDetailsPage() {
    const day = Route.useLoaderData();

    return (
        <section className="card">
            <h2>Detalji dana</h2>

            <p>
                <strong>ID:</strong> {day.id}
            </p>
            <p>
                <strong>Datum:</strong> {formatDate(day.date)}
            </p>
            <p>
                <strong>Osnovna cena:</strong> {day.basePrice}
            </p>

            <div className="actions-row">
                <Link to="/administration/day/$id/edit" params={{ id: String(day.id) }} className="button-link">
                    Izmeni
                </Link>
                <Link to="/administration/day" className="button-link secondary">
                    Nazad
                </Link>
            </div>
        </section>
    );
}