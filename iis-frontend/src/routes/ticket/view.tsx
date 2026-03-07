import { createFileRoute } from '@tanstack/react-router';
import { useState } from 'react';
import { competitionService, ticketService } from '../../api';
import type { GetTicketDto, TicketSelectionDto, TicketErrorDto } from '../../model/ticket';

export const Route = createFileRoute('/ticket/view')({
    loader: async () => competitionService.getCompetition(),
    component: TicketViewPage,
});

function formatDate(value: string) {
    return new Date(value).toLocaleDateString();
}

function TicketViewPage() {
    const competition = Route.useLoaderData();

    const [email, setEmail] = useState('');
    const [ticketCode, setTicketCode] = useState('');

    const [ticket, setTicket] = useState<GetTicketDto | null>(null);
    const [ticketError, setTicketError] = useState<TicketErrorDto | null>(null);
    const [searchError, setSearchError] = useState('');
    const [loading, setLoading] = useState(false);

    const [addSelections, setAddSelections] = useState<TicketSelectionDto[]>(
        competition.days.length > 0 && competition.zones.length > 0
            ? [{ dayId: competition.days[0].id, zoneId: competition.zones[0].id }]
            : [],
    );

    const [removeDayIds, setRemoveDayIds] = useState<number[]>([]);
    const [actionError, setActionError] = useState('');
    const [actionSuccess, setActionSuccess] = useState('');
    const [working, setWorking] = useState(false);

    async function loadTicket() {
        setSearchError('');
        setTicket(null);
        setTicketError(null);
        setActionError('');
        setActionSuccess('');

        if (!email || !ticketCode) {
            setSearchError('Unesi email i šifru karte.');
            return;
        }

        setLoading(true);

        try {
            const found = await ticketService.getByEmailAndCode(email, ticketCode);
            setTicket(found);
            setRemoveDayIds([]);
            return;
        } catch {
            try {
                const err = await ticketService.getError(email, ticketCode);
                setTicketError(err);
            } catch {
                setSearchError('Karta nije pronađena i nema zapisa o grešci obrade.');
            }
        } finally {
            setLoading(false);
        }
    }

    function addSelectionRow() {
        if (competition.days.length === 0 || competition.zones.length === 0) return;

        setAddSelections((prev) => [
            ...prev,
            {
                dayId: competition.days[0].id,
                zoneId: competition.zones[0].id,
            },
        ]);
    }

    function updateAddSelection(index: number, field: 'dayId' | 'zoneId', value: number) {
        setAddSelections((prev) =>
            prev.map((item, i) => (i === index ? { ...item, [field]: value } : item)),
        );
    }

    function removeAddSelectionRow(index: number) {
        setAddSelections((prev) => prev.filter((_, i) => i !== index));
    }

    function toggleRemoveDay(dayId: number) {
        setRemoveDayIds((prev) =>
            prev.includes(dayId) ? prev.filter((x) => x !== dayId) : [...prev, dayId],
        );
    }

    async function handleAdd() {
        setActionError('');
        setActionSuccess('');

        if (!ticket) {
            setActionError('Prvo pronađi kartu.');
            return;
        }

        if (addSelections.length === 0) {
            setActionError('Dodavanje mora sadržati bar jedan dan i zonu.');
            return;
        }

        const dayIds = addSelections.map((s) => s.dayId);
        if (new Set(dayIds).size !== dayIds.length) {
            setActionError('U dodavanju ne možeš navesti isti dan više puta.');
            return;
        }

        setWorking(true);

        try {
            const updated = await ticketService.add({
                email,
                ticketCode,
                add: addSelections,
            });

            setTicket(updated);
            setRemoveDayIds([]);
            setActionSuccess('Karta je uspešno izmenjena.');
        } catch (err) {
            setActionError(err instanceof Error ? err.message : 'Došlo je do greške.');
        } finally {
            setWorking(false);
        }
    }

    async function handleRemove() {
        setActionError('');
        setActionSuccess('');

        if (!ticket) {
            setActionError('Prvo pronađi kartu.');
            return;
        }

        if (removeDayIds.length === 0) {
            setActionError('Izaberi bar jedan dan za uklanjanje.');
            return;
        }

        setWorking(true);

        try {
            const updated = await ticketService.remove({
                email,
                ticketCode,
                removeDayIds,
            });

            setTicket(updated);
            setRemoveDayIds([]);
            setActionSuccess('Dani su uspešno uklonjeni sa karte.');
        } catch (err) {
            setActionError(err instanceof Error ? err.message : 'Došlo je do greške.');
        } finally {
            setWorking(false);
        }
    }

    async function handleCancel() {
        setActionError('');
        setActionSuccess('');

        if (!ticket) {
            setActionError('Prvo pronađi kartu.');
            return;
        }

        const confirmed = window.confirm('Da li sigurno želiš da otkažeš kartu?');
        if (!confirmed) return;

        setWorking(true);

        try {
            await ticketService.cancel({
                email,
                ticketCode,
            });

            setActionSuccess('Karta je uspešno otkazana.');
            await loadTicket();
        } catch (err) {
            setActionError(err instanceof Error ? err.message : 'Došlo je do greške.');
        } finally {
            setWorking(false);
        }
    }

    return (
        <div className="stack">
            <section className="card">
                <h2>Pregled / izmena / otkazivanje karte</h2>

                <div className="form two-columns">
                    <label>
                        Email
                        <input value={email} onChange={(e) => setEmail(e.target.value)} />
                    </label>

                    <label>
                        Šifra karte
                        <input value={ticketCode} onChange={(e) => setTicketCode(e.target.value)} />
                    </label>
                </div>

                <div className="actions-row">
                    <button onClick={loadTicket} disabled={loading}>
                        {loading ? 'Pretraga...' : 'Pronađi kartu'}
                    </button>
                </div>

                {searchError && <div className="alert error">{searchError}</div>}
            </section>

            {ticketError && (
                <section className="card">
                    <h3>Greška obrade zahteva</h3>
                    <p>
                        <strong>Email:</strong> {ticketError.email}
                    </p>
                    <p>
                        <strong>Šifra:</strong> {ticketError.ticketCode}
                    </p>
                    <p>
                        <strong>Poruka:</strong> {ticketError.errorMessage}
                    </p>
                </section>
            )}

            {ticket && (
                <>
                    <section className="card">
                        <h3>Podaci o karti</h3>

                        <p>
                            <strong>Status:</strong> {ticket.status}
                        </p>
                        <p>
                            <strong>Ime i prezime:</strong> {ticket.firstName} {ticket.lastName}
                        </p>
                        <p>
                            <strong>Email:</strong> {ticket.email}
                        </p>
                        <p>
                            <strong>Adresa:</strong> {ticket.addressLine}, {ticket.postalCode}, {ticket.city},{' '}
                            {ticket.country}
                        </p>
                        <p>
                            <strong>Datum kupovine:</strong> {formatDate(ticket.purchaseDate)}
                        </p>
                        <p>
                            <strong>Konačna cena:</strong> {ticket.finalAmount} {ticket.currencyCode}
                        </p>
                        <p>
                            <strong>Kurs:</strong> {ticket.exchangeRate}
                        </p>
                        <p>
                            <strong>Dobijeni promo kod:</strong> {ticket.issuedPromoCode}
                        </p>
                        <p>
                            <strong>Iskorišćen promo kod:</strong> {ticket.usedPromoCode || 'Nije iskorišćen'}
                        </p>

                        <h4>Izabrani dani i zone</h4>
                        {ticket.items.length === 0 ? (
                            <p>Nema stavki.</p>
                        ) : (
                            <table className="table">
                                <thead>
                                    <tr>
                                        <th>Dan</th>
                                        <th>Osnovna cena</th>
                                        <th>Zona</th>
                                        <th>Dodatak zone</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {ticket.items.map((item) => (
                                        <tr key={`${item.dayId}-${item.zoneId}`}>
                                            <td>{formatDate(item.dayDate)}</td>
                                            <td>{item.basePrice}</td>
                                            <td>{item.zoneCharacteristics}</td>
                                            <td>{item.zoneAddon}</td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        )}
                    </section>

                    {actionError && <div className="alert error">{actionError}</div>}
                    {actionSuccess && <div className="alert success">{actionSuccess}</div>}

                    <section className="card">
                        <h3>Dodavanje dana na kartu</h3>

                        {ticket.status !== 'Active' ? (
                            <p>Izmena nije moguća jer karta nije aktivna.</p>
                        ) : (
                            <>
                                <div className="stack">
                                    {addSelections.map((selection, index) => (
                                        <div className="selection-row" key={index}>
                                            <label>
                                                Dan
                                                <select
                                                    value={selection.dayId}
                                                    onChange={(e) =>
                                                        updateAddSelection(index, 'dayId', Number(e.target.value))
                                                    }
                                                >
                                                    {competition.days.map((day) => (
                                                        <option key={day.id} value={day.id}>
                                                            {formatDate(day.date)} - osnovna cena {day.basePrice}
                                                        </option>
                                                    ))}
                                                </select>
                                            </label>

                                            <label>
                                                Zona
                                                <select
                                                    value={selection.zoneId}
                                                    onChange={(e) =>
                                                        updateAddSelection(index, 'zoneId', Number(e.target.value))
                                                    }
                                                >
                                                    {competition.zones.map((zone) => (
                                                        <option key={zone.id} value={zone.id}>
                                                            {zone.characteristics} - kapacitet {zone.capacity} - dodatak {zone.priceAddon}
                                                        </option>
                                                    ))}
                                                </select>
                                            </label>

                                            <button
                                                type="button"
                                                className="danger"
                                                onClick={() => removeAddSelectionRow(index)}
                                                disabled={addSelections.length === 1}
                                            >
                                                Ukloni red
                                            </button>
                                        </div>
                                    ))}
                                </div>

                                <div className="actions-row">
                                    <button type="button" onClick={addSelectionRow}>
                                        Dodaj red
                                    </button>
                                    <button type="button" onClick={handleAdd} disabled={working}>
                                        {working ? 'Obrada...' : 'Dodaj dane'}
                                    </button>
                                </div>
                            </>
                        )}
                    </section>

                    <section className="card">
                        <h3>Uklanjanje dana sa karte</h3>

                        {ticket.status !== 'Active' ? (
                            <p>Izmena nije moguća jer karta nije aktivna.</p>
                        ) : (
                            <>
                                <div className="stack">
                                    {ticket.items.map((item) => (
                                        <label key={item.dayId} className="checkbox-row">
                                            <input
                                                type="checkbox"
                                                checked={removeDayIds.includes(item.dayId)}
                                                onChange={() => toggleRemoveDay(item.dayId)}
                                            />
                                            {formatDate(item.dayDate)} - {item.zoneCharacteristics}
                                        </label>
                                    ))}
                                </div>

                                <div className="actions-row">
                                    <button type="button" onClick={handleRemove} disabled={working}>
                                        {working ? 'Obrada...' : 'Ukloni izabrane dane'}
                                    </button>
                                </div>
                            </>
                        )}
                    </section>

                    <section className="card">
                        <h3>Otkazivanje karte</h3>

                        {ticket.status === 'Cancelled' ? (
                            <p>Karta je već otkazana.</p>
                        ) : (
                            <button type="button" className="danger" onClick={handleCancel} disabled={working}>
                                {working ? 'Obrada...' : 'Otkaži kartu'}
                            </button>
                        )}
                    </section>
                </>
            )}
        </div>
    );
}