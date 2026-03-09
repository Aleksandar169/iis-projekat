import axios from 'axios';


const API_URL = 'https://localhost:7071/api/report'; 

export const getTicketsByRaceDay = () => axios.get(`${API_URL}/tickets-by-race-day`);
export const getPurchasesByDate = () => axios.get(`${API_URL}/purchases-by-date`);