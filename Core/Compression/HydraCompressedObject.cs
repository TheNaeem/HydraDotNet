namespace HydraDotNet.Core.Compression;

public record HydraCompressedObject(HydraCompressionFormat Format, byte[] CompressedData);
