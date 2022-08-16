# WaveFunctionCollapse
Wave function collapse test with a 2D tile set and pre-rotated tiles.

<img width="196" alt="image" src="https://user-images.githubusercontent.com/92940760/184950106-479b7dd0-3043-4ef7-8f3b-aac9e63adc47.png">

[Figure 1] 4x4 Tile set used for WFC.
Each tile gets assigned a 0 or a 1 corresponding to the connection type (0 for connection / yellow, 1 for disconnected / blue). Alongside this each tile has a weight that is used when collapsing a tile to determine which of the possibilities it will generate.

![image](https://user-images.githubusercontent.com/92940760/184950593-6f67815a-4dcc-481e-8538-391bec1ed2f7.png)

[Figure 2] Generation example with equivelant weights for all tiles.

![image](https://user-images.githubusercontent.com/92940760/184950706-7600ad17-e827-4eae-acec-549bdd479a2a.png)

[Figure 3] Generation example with reduced weights of cross tiles and cap tiles.
