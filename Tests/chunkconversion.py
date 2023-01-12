chunkSize = 4096
numChunks = 3

fullSize = chunkSize * numChunks

for i in range(fullSize):
    x = i // 256
    y = i % 256 // 16
    z = i % 16
    print(x, y, z)
