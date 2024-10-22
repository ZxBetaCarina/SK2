using System;
using Random = UnityEngine.Random;

namespace Utility
{
    //General Seed Based 
    public static class SeedBasedRng
    {
        public static int SeedGenerator()
        {
            return (int)System.DateTime.Now.Ticks;
        }

        /// <summary>
        /// Return a random integer from provided range
        /// </summary>
        /// <param name="minInclusive"></param>
        /// <param name="maxExclusive"></param>
        /// <returns></returns>
        public static int IntRNG(int minInclusive, int maxExclusive)
        {
            return Random.Range(minInclusive, maxExclusive);
        }

        /// <summary>
        /// Returns a random float from provided range
        /// </summary>
        /// <param name="minInclusive"></param>
        /// <param name="maxExclusive"></param>
        /// <returns></returns>
        public static float FloatRNG(float minInclusive, float maxExclusive)
        {
            return Random.Range(minInclusive, maxExclusive);
        }

        /// <summary>
        /// Pass a seed to get same result
        /// </summary>
        public static int SeedIntRNG(int min, int max, int? seed = null)
        {
            if (seed != null)
                Random.InitState((int)seed);

            return IntRNG(min, max);

        }

        /// <summary>
        /// Pass a seed to get same result
        /// </summary>
        public static float SeedFloatRNG(float min, float max, int? seed = null)
        {
            if (seed != null)
                Random.InitState((int)seed);

            return FloatRNG(min, max);
        }
    }

    public static class MersenneTwisterRng
    {
        #region constants
        /// <summary>
        /// N
        /// </summary>
        private static readonly int N = 624;

        /// <summary>
        /// M
        /// </summary>
        private static readonly int M = 397;

        /// <summary>
        /// Constant vector a
        /// </summary>
        private static readonly UInt32 MATRIX_A = 0x9908b0df;

        /// <summary>
        /// most significant w-r bits
        /// </summary>
        private static readonly UInt32 UPPER_MASK = 0x80000000;

        /// <summary>
        /// least significant r bits
        /// </summary>
        private static readonly UInt32 LOWER_MASK = 0x7fffffff;

        /// <summary>
        /// Tempering mask B
        /// </summary>
        private static readonly UInt32 TEMPERING_MASK_B = 0x9d2c5680;

        /// <summary>
        /// Tempering mask C
        /// </summary>
        private static readonly UInt32 TEMPERING_MASK_C = 0xefc60000;

        /// <summary>
        /// Last constant used for generation
        /// </summary>
        private static readonly double FINAL_CONSTANT = 2.3283064365386963e-10;
        #endregion


        #region helpers methods
        private static ulong TEMPERING_SHIFT_U(ulong y)
        {
            return y >> 11;
        }

        private static ulong TEMPERING_SHIFT_S(ulong y)
        {
            return y << 7;
        }

        private static ulong TEMPERING_SHIFT_T(ulong y)
        {
            return y << 15;
        }

        private static ulong TEMPERING_SHIFT_L(ulong y)
        {
            return y >> 18;
        }
        #endregion

        #region properties

        /// <summary>
        /// the array for the state vector
        /// </summary>
        private static readonly ulong[] mt = new ulong[625];

        /// <summary>
        /// mti==N+1 means mt[N] is not initialized 
        /// </summary>
        private static int mti = N + 1;
        #endregion

        #region engine
        /// <summary>
        /// setting initial seeds to mt[N]
        /// </summary>
        /// <param name="seed"></param>
        private static void sgenrand(ulong seed)
        {
            mt[0] = seed & 0xffffffff;
            for (mti = 1; mti < N; mti++)
                mt[mti] = (69069 * mt[mti - 1]) & 0xffffffff;
        }

        private static double genrand()
        {
            ulong y;
            ulong[] mag01 = new ulong[2] { 0x0, MATRIX_A };
            /* mag01[x] = x * MATRIX_A  for x=0,1 */

            if (mti >= N)
            { /* generate N words at one time */
                int kk;

                if (mti == N + 1)   /* if sgenrand() has not been called, */
                    sgenrand(4357); /* a default initial seed is used   */

                for (kk = 0; kk < N - M; kk++)
                {
                    y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                    mt[kk] = mt[kk + M] ^ (y >> 1) ^ mag01[y & 0x1];
                }
                for (; kk < N - 1; kk++)
                {
                    y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                    mt[kk] = mt[kk + (M - N)] ^ (y >> 1) ^ mag01[y & 0x1];
                }
                y = (mt[N - 1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
                mt[N - 1] = mt[M - 1] ^ (y >> 1) ^ mag01[y & 0x1];

                mti = 0;
            }

            y = mt[mti++];
            y ^= TEMPERING_SHIFT_U(y);
            y ^= TEMPERING_SHIFT_S(y) & TEMPERING_MASK_B;
            y ^= TEMPERING_SHIFT_T(y) & TEMPERING_MASK_C;
            y ^= TEMPERING_SHIFT_L(y);

            //reals: (0,1)-interval
            //return y; for integer generation
            return ((double)y * FINAL_CONSTANT);
        }
        #endregion

        #region public methods
        /// <summary>
        /// Generate a random number between 0 and 1
        /// </summary>
        /// <returns></returns>
        public static double Generate()
        {
            return genrand();
        }

        /// <summary>
        /// Generate an int between two bounds
        /// </summary>
        /// <param name="lowerBound">The lower bound (inclusive)</param>
        /// <param name="higherBound">The higher bound (inclusive)</param>
        /// <returns></returns>
        public static double Generate(int lowerBound, int higherBound)
        {
            if (higherBound < lowerBound)
            {
                return double.NaN;
            }
            return Convert.ToInt32(Math.Floor(Generate(lowerBound * 1.0d, higherBound * 1.0d)));
        }

        /// <summary>
        /// Generate a double between two bounds
        /// </summary>
        /// <param name="lowerBound">The lower bound (inclusive)</param>
        /// <param name="higherBound">The higher bound (inclusive)</param>
        /// <returns>The random num or NaN if higherbound is lower than lowerbound</returns>
        public static double Generate(double lowerBound, double higherBound)
        {
            if (higherBound < lowerBound)
            {
                return double.NaN;
            }
            return (Generate() * (higherBound - lowerBound + 1)) + lowerBound;
        }
        #endregion
    }
}
