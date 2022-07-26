namespace BROT
{
    public static class BROT
    {
        public static byte[] Rotate(byte[] data, byte rotationDegerees, bool isPositiveRotation = true)
        {
            var resultData = new byte[data.Length];

            for (int offset = 0; offset < data.Length; offset++)
            {
                var resultByte = (short)data[offset];

                if (isPositiveRotation)
                {
                    resultByte &= 0xff;
                    resultByte += rotationDegerees;
                }
                else
                {
                    resultByte |= 0x100;
                    resultByte -= rotationDegerees;
                }

                resultByte &= 0xff;
                resultData[offset] = (byte)resultByte;
            }

            return resultData;
        }
    }
}
