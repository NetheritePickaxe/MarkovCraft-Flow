#nullable enable
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace MarkovCraft
{
    public static class McFuncExporter
    {
        private static bool IsAir(string blockState) => string.IsNullOrWhiteSpace(blockState) || blockState.Equals("minecraft:air");

        public static void Export(int SizeX, int SizeY, int SizeZ, CustomMappingItem[] resultPalette,
                int[] blockData, string filePath, bool fillMerge = true, bool cullAir = true)
        {
            int mcSizeX = SizeZ;
            int mcSizeY = SizeY;
            int mcSizeZ = SizeX;

            int GetIndex(int mcx, int mcy, int mcz) => mcz + mcx * mcSizeZ + mcy * mcSizeZ * mcSizeX;

            var funcText = new StringBuilder();
            int count = mcSizeX * mcSizeY * mcSizeZ;
            bool[]? processed = fillMerge ? new bool[count] : null;

            for (int mcy = 0; mcy < mcSizeY; mcy++)
            {
                for (int mcx = 0; mcx < mcSizeX; mcx++)
                {
                    for (int mcz = 0; mcz < mcSizeZ; mcz++)
                    {
                        int index = GetIndex(mcx, mcy, mcz);
                        int resultIndex = blockData[index];
                        var blockState = resultPalette[resultIndex].BlockState;

                        if (cullAir && IsAir(blockState)) continue;
                        if (fillMerge && processed![index]) continue;

                        if (fillMerge)
                        {
                            int maxX = mcx, maxY = mcy, maxZ = mcz;

                            // Expand along X
                            for (int x = mcx + 1; x < mcSizeX; x++)
                            {
                                int idx = GetIndex(x, mcy, mcz);
                                if (blockData[idx] != resultIndex || IsAir(resultPalette[blockData[idx]].BlockState) || processed![idx]) break;
                                maxX = x;
                            }

                            // Expand along Y
                            bool canExtendY = true;
                            for (int y = mcy + 1; y < mcSizeY && canExtendY; y++)
                            {
                                for (int x = mcx; x <= maxX; x++)
                                {
                                    int idx = GetIndex(x, y, mcz);
                                    if (blockData[idx] != resultIndex || IsAir(resultPalette[blockData[idx]].BlockState) || processed![idx])
                                    {
                                        canExtendY = false;
                                        break;
                                    }
                                }
                                if (canExtendY) maxY = y;
                            }

                            // Expand along Z
                            bool canExtendZ = true;
                            for (int z = mcz + 1; z < mcSizeZ && canExtendZ; z++)
                            {
                                for (int x = mcx; x <= maxX; x++)
                                for (int y = mcy; y <= maxY; y++)
                                {
                                    int idx = GetIndex(x, y, z);
                                    if (blockData[idx] != resultIndex || IsAir(resultPalette[blockData[idx]].BlockState) || processed![idx])
                                    {
                                        canExtendZ = false;
                                        break;
                                    }
                                }
                                if (canExtendZ) maxZ = z;
                            }

                            // Mark processed
                            for (int x = mcx; x <= maxX; x++)
                            for (int y = mcy; y <= maxY; y++)
                            for (int z = mcz; z <= maxZ; z++)
                                processed![GetIndex(x, y, z)] = true;

                            string from = $"~{RelCoord(mcx)} ~{RelCoord(mcy)} ~{RelCoord(mcz)}";
                            string to = $"~{RelCoord(maxX)} ~{RelCoord(maxY)} ~{RelCoord(maxZ)}";
                            funcText.AppendLine($"fill {from} {to} {blockState}");
                        }
                        else
                        {
                            funcText.AppendLine($"setblock ~{RelCoord(mcx)} ~{RelCoord(mcy)} ~{RelCoord(mcz)} {blockState}");
                        }
                    }
                }
            }

            File.WriteAllText(filePath, funcText.ToString());
            Debug.Log($"McFunction file exported to {filePath}");
        }

        private static string RelCoord(int value) => value == 0 ? "~" : $"~{value}";
    }
}