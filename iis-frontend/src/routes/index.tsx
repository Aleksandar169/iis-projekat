import { createFileRoute, Link } from '@tanstack/react-router';
import { competitionService } from '../api';

export const Route = createFileRoute('/')({
  loader: async () => {
    return competitionService.tryGetCompetition();
  },
  component: HomePage,
});

function formatDate(value: string) {
  return new Date(value).toLocaleDateString();
}

function HomePage() {
  const competition = Route.useLoaderData();

  if (!competition) {
    return (
      <div className="card">
        <h2>Početna stranica</h2>
        <p>Osnovne informacije o takmičenju još nisu podešene.</p>
        <Link to="/administration" className="button-link">
          Idi na administraciju
        </Link>
      </div>
    );
  }

  return (
    <div className="stack">
      <section className="card">
        <h2>{competition.name}</h2>
        <p>
          <strong>Lokacija:</strong> {competition.location}
        </p>
        <p>
          <strong>Datum održavanja:</strong> {formatDate(competition.startDate)} -{' '}
          {formatDate(competition.endDate)}
        </p>
        <p>
          <strong>Popust 10% važi do:</strong> {formatDate(competition.discountValidUntil)}
        </p>
        <p>
          <strong>Dodatne informacije:</strong>{' '}
          {competition.additionalInfo?.trim() || 'Nema dodatnih informacija.'}
        </p>
      </section>

      <section className="card">
        <h3>Dani takmičenja</h3>
        {competition.days.length === 0 ? (
          <p>Nema unetih dana.</p>
        ) : (
          <table className="table">
            <thead>
              <tr>
                <th>Datum</th>
                <th>Osnovna cena</th>
              </tr>
            </thead>
            <tbody>
              {competition.days.map((day) => (
                <tr key={day.id}>
                  <td>{formatDate(day.date)}</td>
                  <td>{day.basePrice}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </section>

      <section className="card">
        <h3>Zone sedenja</h3>
        {competition.zones.length === 0 ? (
          <p>Nema unetih zona.</p>
        ) : (
          <table className="table">
            <thead>
              <tr>
                <th>Karakteristike</th>
                <th>Kapacitet</th>
                <th>Dodatak na cenu</th>
              </tr>
            </thead>
            <tbody>
              {competition.zones.map((zone) => (
                <tr key={zone.id}>
                  <td>{zone.characteristics}</td>
                  <td>{zone.capacity}</td>
                  <td>{zone.priceAddon}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </section>

      <section className="card">
        <h3>Dozvoljene valute</h3>
        {competition.allowedCurrencies.length === 0 ? (
          <p>Nema podešenih valuta.</p>
        ) : (
          <ul>
            {competition.allowedCurrencies.map((currency) => (
              <li key={currency.id}>
                {currency.currencyName} ({currency.code})
              </li>
            ))}
          </ul>
        )}
      </section>

      <section className="card actions-row">
        <Link to="/ticket/new" className="button-link">
          Kupi kartu
        </Link>
        <Link to="/ticket/view" className="button-link secondary">
          Pregled / izmena / otkazivanje
        </Link>
      </section>
    </div>
  );
}