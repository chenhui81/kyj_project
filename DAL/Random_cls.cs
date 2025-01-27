using System;
using System.Threading;

namespace kyj_project.DAL
{
    public class Random_cls
    {

        /// <summary>
        /// 使用RNGCryptoServiceProvider生成种子
        /// </summary>
        /// <returns></returns>
        public static int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);

        }
        /// <summary>
        /// 使用Guid生成种子
        /// </summary>
        /// <returns></returns>
        static int GetRandomSeedbyGuid()
        {
            return Guid.NewGuid().GetHashCode();
        }

        /// <summary>
        /// 默认方式产生随机数种子
        /// </summary>
        static void GetRandomDefault(int[] array)
        {
            int len = array.Length;
            Random random = new Random();

            for (int i = 0; i < len; i++)
            {

                array[i] = random.Next(0, len);
            }
            //Print(array);// 输出生成的随机数
        }
        /// <summary>
        /// 使用Thread.Sleep()方式产生真随机数
        /// </summary>
        static void GetRandomBySleep(int[] array)
        {
            int len = array.Length;
            Random random = new Random();

            for (int i = 0; i < len; i++)
            {
                Thread.Sleep(1);
                array[i] = random.Next(0, len);
            }
            //Print(array);// 输出生成的随机数
        }
        /// <summary>
        /// 使用RNGCryptoServiceProvider产生的种子生成真随机数
        /// </summary>
        static void GetRandomByRNGCryptoServiceProvider(int[] array)
        {
            int len = array.Length;
            Random random = new Random(GetRandomSeed());
            for (int i = 0; i < len; i++)
            {

                array[i] = random.Next(0, len);
            }
            //Print(array);// 输出生成的随机数
        }
        /// <summary>
        /// 使用Guid产生的种子生成真随机数
        /// </summary>
        static void GetRandomByGuid(int[] array)
        {
            int len = array.Length;
            Random random = new Random(GetRandomSeedbyGuid());
            for (int i = 0; i < len; i++)
            {
                array[i] = random.Next(0, len);
            }
            //Print(array);// 输出生成的随机数
        }
        static void Print(int[] array)
        {
            for (int i = 0; i < array.Length; i++)
                Console.Write(string.Format("{0} ", array[i]));
            Console.ReadLine();
        }
    }


}
