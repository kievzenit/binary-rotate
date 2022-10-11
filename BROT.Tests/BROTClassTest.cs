namespace BROT.Tests
{
    [TestClass]
    public class BROTClassTest
    {
        const byte ROTATION_DEGREE = 13;

        [TestMethod]
        public void PositiveRotate_255_With_13degrees_ShouldBe_12()
        {
            //arrange
            var data = new byte[1] { 255 };
            const byte expected = 12;
            const bool isPositiveRotation = true;

            //act
            var actual = BROT.Utils.BROT.Rotate(data, ROTATION_DEGREE, isPositiveRotation);

            //assert
            Assert.AreEqual(expected, actual[0]);
        }

        [TestMethod]
        public void PositiveRotate_0_With_13degrees_ShouldBe_13()
        {
            //arrange
            var data = new byte[1] { 0 };
            const byte expected = 13;
            const bool isPositiveRotation = true;

            //act
            var actual = BROT.Utils.BROT.Rotate(data, ROTATION_DEGREE, isPositiveRotation);

            //assert
            Assert.AreEqual(expected, actual[0]);
        }

        [TestMethod]
        public void NegativeRotate_12_With_13degrees_ShouldBe_255()
        {
            //arrange
            var data = new byte[1] { 12 };
            const byte expected = 255;
            const bool isPositiveRotation = false;

            //act
            var actual = BROT.Utils.BROT.Rotate(data, ROTATION_DEGREE, isPositiveRotation);

            //assert
            Assert.AreEqual(expected, actual[0]);
        }

        [TestMethod]
        public void NegativeRotate_13_With_13degrees_ShouldBe_0()
        {
            //arrange
            var data = new byte[1] { 13 };
            const byte expected = 0;
            const bool isPositiveRotation = false;

            //act
            var actual = BROT.Utils.BROT.Rotate(data, ROTATION_DEGREE, isPositiveRotation);

            //assert
            Assert.AreEqual(expected, actual[0]);
        }
    }
}