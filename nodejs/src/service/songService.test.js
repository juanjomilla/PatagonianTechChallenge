const proxyquire = require('proxyquire');
const sinon = require('sinon');
const test = require('unit.js');

describe('Song Service Unit Test', () => {
  afterEach(() => {
    sinon.restore();
  });

  it('If the song exists, should return true', async () => {
    // Arrange
    const songsDaoGetMock = sinon.stub().resolves(1);
    const songsService = proxyquire('../service/songService', {
      '../dao/songsDao': { get: songsDaoGetMock }
    });

    const expectedSQLQuery = 'SELECT 1 FROM track WHERE id=?';

    // Act
    const result = await songsService.songExists('song');

    // Assert
    test.value(result).isEqualTo(true);
    sinon.assert.calledWithExactly(songsDaoGetMock, expectedSQLQuery, ['song']);
  });

  it('getCountSongsByArtist should return the total songs', async () => {
    // Arrange
    const songsDaoGetMock = sinon.stub().resolves(999);
    const songsService = proxyquire('../service/songService', {
      '../dao/songsDao': { get: songsDaoGetMock }
    });

    const expectedSQLQuery = 'SELECT COUNT(track.id) as total FROM track INNER JOIN artist ON track.artist_id=artist.id WHERE artist.name LIKE \'%artistName%\'';

    // Act
    const result = await songsService.getCountSongsByArtist('artistName');

    // Assert
    test.value(result).isEqualTo(999);
    sinon.assert.calledWithExactly(songsDaoGetMock, expectedSQLQuery);
  });

  it('getSongInfo should return songInfo', async () => {
    // Arrange
    const songInfoMock = {
      disc_number: 1,
      duration_ms: 275386,
      id: '0dEIca2nhcxDUV8C5QkPYb',
      name: 'Give Life Back to Music'
    };
    const songsDaoGetMock = sinon.stub().resolves(songInfoMock);
    const songsService = proxyquire('../service/songService', {
      '../dao/songsDao': { getSongInfo: songsDaoGetMock }
    });

    // Act
    const result = await songsService.getSongInfo('0dEIca2nhcxDUV8C5QkPYb');

    // Assert
    test.value(result.id).isEqualTo('0dEIca2nhcxDUV8C5QkPYb');
    test.value(result.name).isEqualTo('Give Life Back to Music');
    test.value(result.disc_number).isEqualTo(1);
    test.value(result.duration_ms).isEqualTo(275386);
  });

  it('getAllSongsByArtist should return all songs by artist', async () => {
    // Arrange
    const allSongsMock = [
      {
        songName: 'Give Life Back to Music',
        songId: '0dEIca2nhcxDUV8C5QkPYb'
      },
      {
        songName: 'The Game of Love',
        songId: '3ctALmweZBapfBdFiIVpji'
      }
    ];

    const songsDaoGetMock = sinon.stub().resolves(allSongsMock);
    const songsService = proxyquire('../service/songService', {
      '../dao/songsDao': { getAll: songsDaoGetMock }
    });

    const expectedSQLQuery = 'SELECT track.id AS songId, track.name AS songTitle FROM track INNER JOIN artist ON track.artist_id=artist.id WHERE artist.name LIKE \'%artistName%\' LIMIT 2, 4';

    // Act
    const result = await songsService.getAllSongsByArtist('artistName', 4, 2);

    // Assert
    test.value(result).isEqualTo(allSongsMock);
    sinon.assert.calledWithExactly(songsDaoGetMock, expectedSQLQuery);
  });
});
