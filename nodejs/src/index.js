const songsRoutes =  require('./routes/songs');
const express = require('express');

const app = express();

const appPort = process.env.PORT || 32964;
app.use(express.json());

app.use('/api/v1', songsRoutes);

app.listen(appPort, () => {
  console.log(`Server listening on port ${appPort}`);
});
