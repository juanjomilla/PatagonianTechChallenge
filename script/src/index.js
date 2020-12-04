const axios = require('axios').default;
const sqlite3 = require('sqlite3');
const args = require('minimist')(process.argv.slice(2));

const { clientId, secretKey, artistIds, projectType } = args;
const spotifyBaseApiUrl = 'https://api.spotify.com/v1';

const databasePath = getDatabasePath();
const db = initializeDatabase(databasePath);
let authorizationToken = '';

async function getAuthorizationToken() {
  const authURL = 'https://accounts.spotify.com/api/token';
  const auth = 'Basic ' + Buffer.from(clientId + ':' + secretKey).toString('base64');

  const authResponse = await axios.post(
    authURL,
    'grant_type=client_credentials', {
    headers: {
      Authorization: auth
    }
  });

  return authResponse.data.access_token;
};

async function getArtistName(artistId) {
  const url = `${spotifyBaseApiUrl}/artists/${artistId}`;

  try {
    const request = await retryGet(url, {
      headers: {
        Authorization: `Bearer ${authorizationToken}`
      }
    });

    return request.data.name;
  } catch (err) {
    console.error('An error has ocurred', err);
  }
}

async function getArtistIdsAlbums(artistId) {
  const albumsIds = [];
  let url = `https://api.spotify.com/v1/artists/${artistId}/albums?limit=50&include_groups=album`;

  do {
    const request = await retryGet(url, {
      headers: {
        Authorization: `Bearer ${authorizationToken}`
      }
    });

    const albums = request.data.items;
    albums.map((album) => {
      albumsIds.push(album.id);
    });

    url = request.data.next;

  } while (url)

  return albumsIds;
}

async function getArtistTracks(artistId) {
  try {
    const promiseArray = [];
    const artistName = await getArtistName(artistId);
    const artistAlbums = await getArtistIdsAlbums(artistId);

    console.log(`Getting tracks for ${artistName}...`);
    artistAlbums.forEach(album => promiseArray.push(getTrackByAlbumId(album)));
    const tracks = await Promise.all(promiseArray);

    return { artistName, artistId: artistId, tracks: tracks.flat() };
  } catch (err) {
    console.log(err)
  }
}

async function getTrackByAlbumId(albumId) {
  const tracksNames = [];
  let url = `https://api.spotify.com/v1/albums/${albumId}/tracks`;

  do {
    const request = await retryGet(url, {
      headers: {
        Authorization: `Bearer ${authorizationToken}`
      }
    })

    const tracks = request.data.items;

    tracks.map((track) => {
      tracksNames.push({ name: track.name, id: track.id });
    });

    url = request.data.next;

  } while (url)

  return tracksNames;
}

function getDatabasePath(){
  if (projectType === 'netcore'){
    return '../netcore/PatagonianChallengeAPI/PatagonianChallengeAPI/database/dbtest.db';
  }

  return '../nodejs/database/dbtest.db';
}

async function retryGet(url, config, maxRetries = 5) {
  try {
    return await axios.get(url, config);
  } 
  catch (error) {
    if (error.response.status == 429 && maxRetries > 0) {
      const retryAfter = parseInt(error.response.headers['retry-after']) || 30;

      console.log(`Retrying in ${retryAfter} seconds due to 429 status...`);
      await wait(retryAfter);

      return retryGet(url, config, maxRetries - 1)
    }

    throw error;
  }
}

function initializeDatabase(databasePath) {
  const db = new sqlite3.Database(databasePath, sqlite3.OPEN_READWRITE | sqlite3.OPEN_CREATE, err => {
    if (err) {
      console.error('An error has ocurred when connecting to the database: ', err);
      throw new Error('An error has ocurred open when connecting to the database');
    }
  });

  db.serialize(() => {
    db.run('DROP TABLE IF EXISTS artist')
      .run('CREATE TABLE artist (id TEXT NOT NULL, name TEXT NOT NULL, PRIMARY KEY(id))')
      .run('DROP TABLE IF EXISTS track')
      .run('CREATE TABLE track (id TEXT NOT NULL, name TEXT NOT NULL, artist_id TEXT NOT NULL, FOREIGN KEY(artist_id) REFERENCES artist(id), PRIMARY KEY(id))');
  });

  return db;
}

async function wait(seconds) {
  return new Promise((resolve, reject) => setTimeout(resolve, seconds * 1000));
}

function saveArtistIntoDatabase(artistId, artistName) {
  return new Promise((resolve, reject) => {
    db.run('INSERT INTO artist(id, name) VALUES (?, ?)', [artistId, artistName], (err) => {
      if (err) reject(err);
      else resolve(this);
    });
  });
}

function saveTrackIntoDatabase(trackId, trackName, artistId) {
  return new Promise((resolve, reject) => {
    db.run('INSERT INTO track(id, name, artist_id) VALUES (?, ?, ?)', [trackId, trackName, artistId], (err) => {
      if (err) reject(err);
      else resolve(this);
    });
  });
}

async function saveResultsIntoDatabase(results) {
  for (const result of results) {
    await saveArtistIntoDatabase(result.artistId, result.artistName);

    for (const track of result.tracks) {
      await saveTrackIntoDatabase(track.id, track.name, result.artistId);
    }
  }
}

function verifyArguments() {
  if (!clientId || !secretKey || !artistIds) {
    throw new Error('Missing required argument. Please read script documentation.');
  }
}

async function main() {
  try {
    verifyArguments();
    const promiseArray = [];
    const artistIdsArray = artistIds.split(',');
    authorizationToken = await getAuthorizationToken();

    artistIdsArray.forEach(artistId => promiseArray.push(getArtistTracks(artistId)));

    const results = await Promise.all(promiseArray)

    console.log('Saving results into database...');
    await saveResultsIntoDatabase(results);
    console.log('Saved!');
  } catch (err) {
    console.error('An error has ocurred when running the script.', err);
  }
}

main();
