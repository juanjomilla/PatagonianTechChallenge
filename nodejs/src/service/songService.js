const songsDao = require('../dao/songsDao');

exports.getCountSongsByArtist = async (artistName) => {
  return songsDao.get(
    `SELECT COUNT(track.id) as total FROM track INNER JOIN artist ON track.artist_id=artist.id WHERE artist.name LIKE '%${artistName}%'`
  );
};

exports.getAllSongsByArtist = async (artistName, limit, offset) => {
  return songsDao.getAll(
    `SELECT track.id AS songId, track.name AS songTitle FROM track INNER JOIN artist ON track.artist_id=artist.id WHERE artist.name LIKE '%${artistName}%' LIMIT ${offset}, ${limit}`
  );
};

exports.songExists = async (songId) => {
  const song = await songsDao.get('SELECT 1 FROM track WHERE id=?', [songId]);

  return song != undefined;
};

exports.getSongInfo = async (songId) => {
  return songsDao.getSongInfo(songId);
}
