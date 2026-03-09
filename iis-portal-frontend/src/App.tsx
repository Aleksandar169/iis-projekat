import React, { useEffect, useState } from 'react';
import { BarChart, Bar, XAxis, YAxis, Tooltip, LineChart, Line, CartesianGrid, ResponsiveContainer } from 'recharts';
import { getTicketsByRaceDay, getPurchasesByDate } from './services/reportService';

function App() {
  const [raceDayData, setRaceDayData] = useState([]);
  const [purchaseDateData, setPurchaseDateData] = useState([]);

  useEffect(() => {
    getTicketsByRaceDay().then(res => setRaceDayData(res.data));
    getPurchasesByDate().then(res => setPurchaseDateData(res.data));
  }, []);

  return (
    <div style={{ padding: '40px', backgroundColor: '#f5f5f5', minHeight: '100vh' }}>
      <h1>F1 Reporting Portal - Dashboard</h1>
      
      <div style={{ display: 'flex', gap: '20px', flexWrap: 'wrap', marginTop: '30px' }}>
        
     
        <div style={{ background: 'white', padding: '20px', borderRadius: '8px', boxShadow: '0 2px 4px rgba(0,0,0,0.1)' }}>
          <h3>Broj kupljenih karata po danima takmičenja</h3>
          <BarChart width={500} height={300} data={raceDayData}>
            <XAxis dataKey="day" />
            <YAxis />
            <Tooltip />
            <Bar dataKey="count" fill="#8884d8" name="Broj karata" />
          </BarChart>
        </div>

       
        <div style={{ background: 'white', padding: '20px', borderRadius: '8px', boxShadow: '0 2px 4px rgba(0,0,0,0.1)' }}>
          <h3>Broj kupljenih karata po datumima kupovine</h3>
          <LineChart width={500} height={300} data={purchaseDateData}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis dataKey="date" />
            <YAxis />
            <Tooltip />
            <Line type="monotone" dataKey="count" stroke="#82ca9d" name="Broj kupovina" />
          </LineChart>
        </div>

      </div>
    </div>
  );
}

export default App;
