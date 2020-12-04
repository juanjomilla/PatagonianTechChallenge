const songsService = require('../service/songService');

const express = require('express');
const router = express.Router();

router.get('/songs/:id', async (req, res) => {
  const songId = req.params.id;

  if (!songsService.songExists(songId)) {
    res.status(404).json({ message: 'Song not found' });
  } else {
    const songInfo = await songsService.getSongInfo(songId);
    res.status(200).json(songInfo);
  }
});

router.get('/songs', async (req, res) => {
  const limit = parseInt(req.query.limit) || 20;
  const offset = parseInt(req.query.offset) || 0;
  const artistName = req.query.artistName;

  if (artistName == undefined || artistName.length < 3) {
    res.status(400).send('The \'artistName\' parameter is mandatory, should be not empty and should have at least 3 characters');
  } else {
    const totalSongs = await songsService.getCountSongsByArtist(artistName);
    const songs = await songsService.getAllSongsByArtist(artistName, limit, offset);

    const nextUrl = totalSongs.total >= offset + limit + 1 ? buildNextUrl(artistName, limit, offset, req) : undefined;

    res.status(200).json({ songs, next: nextUrl });
  }
});

function buildNextUrl(artistName, limit, offset, req) {
  const fullUrl = req.protocol + '://' + req.get('host');

  return `${fullUrl}/api/v1/songs?artistName=${artistName}&offset=${offset + limit}&limit=${limit}`;
}

module.exports = router;
