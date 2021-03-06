﻿using System;
using System.Collections.Generic;
using System.Linq;
using AtCoderTemplateForNetCore.Numerics;
using Xunit;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Numerics
{
    public class ErathostenesTest
    {
        static Eratosthenes _eratosthenes = new Eratosthenes(Max);
        const int Max = 1_000_000;

        [Theory]
        [InlineData(1)]
        [InlineData(2, 2)]
        [InlineData(9, 3, 3)]
        [InlineData(10, 2, 5)]
        [InlineData(111, 3, 37)]
        [InlineData(13500, 2, 2, 3, 3, 3, 5, 5, 5)]
        [InlineData(456480, 2, 2, 2, 2, 2, 3, 3, 5, 317)]
        [InlineData(524288, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2)]
        [InlineData(606449, 606449)]
        [InlineData(893810, 2, 5, 89381)]
        public void PrimeFactorizeTest(int n, params int[] primes)
        {
            var result = new List<int>();
            foreach (var (prime, count) in _eratosthenes.PrimeFactorize(n))
            {
                for (int i = 0; i < count; i++)
                {
                    result.Add(prime);
                }
            }

            Assert.Equal(primes, result);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, false)]
        [InlineData(2, true)]
        [InlineData(5, true)]
        [InlineData(15, false)]
        [InlineData(13500, false)]
        [InlineData(606449, true)]
        public void IsPrimeTest(int n, bool isPrime) => Assert.Equal(isPrime, _eratosthenes.IsPrime(n));

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 1, 2)]
        [InlineData(60, 1, 2, 3, 4, 5, 6, 10, 12, 15, 20, 30, 60)]
        [InlineData(746496, 1, 2, 3, 4, 6, 8, 9, 12, 16, 18, 24, 27, 32, 36, 48, 54, 64, 72, 81, 96, 108, 128, 144, 162, 192, 216, 243, 256, 288, 324, 384, 432, 486, 512, 576, 648, 729, 768, 864, 972, 1024, 1152, 1296, 1458, 1536, 1728, 1944, 2304, 2592, 2916, 3072, 3456, 3888, 4608, 5184, 5832, 6912, 7776, 9216, 10368, 11664, 13824, 15552, 20736, 23328, 27648, 31104, 41472, 46656, 62208, 82944, 93312, 124416, 186624, 248832, 373248, 746496)]
        [InlineData(810000, 1, 2, 3, 4, 5, 6, 8, 9, 10, 12, 15, 16, 18, 20, 24, 25, 27, 30, 36, 40, 45, 48, 50, 54, 60, 72, 75, 80, 81, 90, 100, 108, 120, 125, 135, 144, 150, 162, 180, 200, 216, 225, 240, 250, 270, 300, 324, 360, 375, 400, 405, 432, 450, 500, 540, 600, 625, 648, 675, 720, 750, 810, 900, 1000, 1080, 1125, 1200, 1250, 1296, 1350, 1500, 1620, 1800, 1875, 2000, 2025, 2160, 2250, 2500, 2700, 3000, 3240, 3375, 3600, 3750, 4050, 4500, 5000, 5400, 5625, 6000, 6480, 6750, 7500, 8100, 9000, 10000, 10125, 10800, 11250, 13500, 15000, 16200, 16875, 18000, 20250, 22500, 27000, 30000, 32400, 33750, 40500, 45000, 50625, 54000, 67500, 81000, 90000, 101250, 135000, 162000, 202500, 270000, 405000, 810000)]
        [InlineData(450450, 1, 2, 3, 5, 6, 7, 9, 10, 11, 13, 14, 15, 18, 21, 22, 25, 26, 30, 33, 35, 39, 42, 45, 50, 55, 63, 65, 66, 70, 75, 77, 78, 90, 91, 99, 105, 110, 117, 126, 130, 143, 150, 154, 165, 175, 182, 195, 198, 210, 225, 231, 234, 273, 275, 286, 315, 325, 330, 350, 385, 390, 429, 450, 455, 462, 495, 525, 546, 550, 585, 630, 650, 693, 715, 770, 819, 825, 858, 910, 975, 990, 1001, 1050, 1155, 1170, 1287, 1365, 1386, 1430, 1575, 1638, 1650, 1925, 1950, 2002, 2145, 2275, 2310, 2475, 2574, 2730, 2925, 3003, 3150, 3465, 3575, 3850, 4095, 4290, 4550, 4950, 5005, 5775, 5850, 6006, 6435, 6825, 6930, 7150, 8190, 9009, 10010, 10725, 11550, 12870, 13650, 15015, 17325, 18018, 20475, 21450, 25025, 30030, 32175, 34650, 40950, 45045, 50050, 64350, 75075, 90090, 150150, 225225, 450450)]
        [InlineData(606449, 1, 606449)]
        public void GetDivisiorsTest(int n, params int[] divisiors)
        {
            Assert.Equal(divisiors, _eratosthenes.GetDivisiors(n).OrderBy(i => i));
        }
    }
}
