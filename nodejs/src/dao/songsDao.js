const sqlite3 = require('sqlite3');
const axios = require('axios').default;
require('dotenv').config({ path: process.cwd() + '/.env' });

const secretKey = process.env.SPOTIFY_SECRET_KEY;
const clientId = process.env.SPOTIFY_CLIENT_ID;

const db = new sqlite3.Database('./database/dbtest.db', sqlite3.OPEN_READWRITE, (err) => {
  if (err) {
    console.error('An error has ocurred connecting to the database', err);
    throw new Error('An error has ocurred connecting to the database');
  }
});

exports.getAll = (sqlQuery, ...params) => {
  return new Promise((resolve, reject) => {
    db.all(sqlQuery, params, (err, rows) => {
      if (err) reject(err);
      else resolve(rows);
    });
  });
}

exports.get = (sqlQuery, ...params) => {
  return new Promise((resolve, reject) => {
    db.get(sqlQuery, params, (err, row) => {
      if (err) reject(err);
      else resolve(row);
    });
  });
}

exports.getSongInfo = async (songId) => {
  const url = `https://api.spotify.com/v1/tracks/${songId}`;
  const authorizationToken = await getAuthorizationToken();
  
  const result = await axios.get(url, {
    headers: {
      Authorization: `Bearer ${authorizationToken}`
    }
  });
  
  const songInfo = result.data;
  delete songInfo.album;
  delete songInfo.available_markets;
  
  return songInfo;
}

async function getAuthorizationToken() {
  const authURL = 'https://accounts.spotify.com/api/token';
  const auth = 'Basic ' + Buffer.from(clientId + ':' + secretKey).toString('base64');
  try {
    const authResponse = await axios.post(
      authURL,
      'grant_type=client_credentials', {
      headers: {
        Authorization: auth
      }
    });

    return authResponse.data.access_token;
  } catch (error) {
    console.error(error);
  }
};
