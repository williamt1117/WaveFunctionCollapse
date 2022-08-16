# WaveFunctionCollapse
Wave function collapse test with a 2D tile set and pre-rotated tiles. Developed in Unity and uses the language C#.

<img width="191" alt="image" src="https://user-images.githubusercontent.com/92940760/184951534-8e26a17c-95d3-4b3e-9422-58a8030733d3.png">

[Figure 1] 4x4 Tile set used for WFC.


Each tile gets assigned a 0 or a 1 corresponding to the connection type (0 for connection / yellow, 1 for disconnected / blue). Alongside this each tile has a weight that is used when collapsing a tile to determine which of the possibilities it will generate.

![image](https://user-images.githubusercontent.com/92940760/184950593-6f67815a-4dcc-481e-8538-391bec1ed2f7.png)

[Figure 2] Generation example with equivelant weights for all tiles.

![image](https://user-images.githubusercontent.com/92940760/184950706-7600ad17-e827-4eae-acec-549bdd479a2a.png)

[Figure 3] Generation example with reduced weights of cross tiles and cap tiles.

<img width="238" alt="image" src="https://user-images.githubusercontent.com/92940760/184951846-8546e261-fa1a-4640-b0db-a41b0594c755.png">
[Figure 4] Generation example removing blank tiles and increasing weight of corner and straight tiles.
