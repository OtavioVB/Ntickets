import http from 'k6/http';

export const options = {
    vus: 10,
    duration: '60s',
    thresholds: {
        http_req_failed: ['rate<0.01'],
        http_req_duration: ['p(90)<500'], 
    },
};

function getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

export default function () {
    const email = `${getRandomInt(10000000000,99999999999).toString()}@ntickets.com.br`;
    let data = {
        email: email,
        fantasyName: 'Eventos',
        legalName: 'Eventos LTDA',
        phone: '5511999999090',
        document: getRandomInt(10000000000,99999999999).toString()
    };

    http.post('http://localhost:5001/api/v1/business-intelligence/tenants', JSON.stringify(data), {
        headers: {
            'Content-Type': 'application/json',
            'X-Correlation-Id': `${getRandomInt(10000000000,99999999999).toString()}`
        }
    });
}
