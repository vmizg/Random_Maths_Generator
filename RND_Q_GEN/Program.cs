using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*  Written by Vytautas Mizgiris.
    Simple maths sum question and options generator.

    IMPORTANT NOTES & ASSUMPTIONS:

    * The answers to every question (INCLUDING wrong ones) are chosen in the range of the right answer
    (i.e. between 1 and 10, 11 and 20, 21 and 100).
    
    * Weights of the wrong answers are assigned array-wise: the lower position they have in the variants[] array,
    the higher priority they have when choosing between them randomly. There are specific bounds for all three
    wrong variants, i.e. highest priority answers will lie between 1st and 10th elements in the array, lower -
    between 11th and 20th, and so on.

    * I used a script, which implements a random number generator algorithm called MersenneTwister,
    which is possibly better than just the built-in Random library - I still am aware that it is
    very far from being useful. I did some research and it appears that the website random.org (generating
    random numbers from atmospheric noise) offers a free to use HTTP GET API, though I'm not very familiar
    with integrating HTTP GET requests in C# and will stick to the exercise itself.
*/

namespace RND_Q_GEN
{
    class Program
    {
        // MAIN METHOD FOR TESTING
        // the contents can be modified to see how the program performs

        static void Main(string[] args)
        {
            // To test generation of questions pass values such as:
            // For Q1: (1, 10)
            // For Q2: (11, 19)
            // For Q3: (21, 99)

            Questions generator = new Questions();
            System.Diagnostics.Debug.WriteLine(generator.Question_Randomizer(1, 10));
            System.Diagnostics.Debug.WriteLine(generator.Question_Randomizer(11, 19));
            System.Diagnostics.Debug.WriteLine(generator.Question_Randomizer(21, 99));
        }
    }

    class Questions
    {
        private Int32[] r_num;
        private Int32[] variants;
        private Int32[] final_options;
        private Int32 min, max;
        private Int32 test_num;

        public Questions()
        {
            r_num = new Int32[2];
            variants = new Int32[100];
            final_options = new Int32[4];
            test_num = 0;
        }

        public String Question_Randomizer(Int32 min, Int32 max)
        {
            this.min = min;
            this.max = max;

            MersenneTwister t = new MersenneTwister();

            r_num[0] = t.Next(1, max);
            while (r_num[0] == test_num) r_num[0] = t.Next(1, max);
            test_num = r_num[0];

            // If the number we get at random is lower than our minimum sum value,
            // then we need to calculate the minimum possible value for the other summand:

            if (r_num[0] < min) 
                r_num[1] = t.Next(min - r_num[0], max - r_num[0] + 1);
            else if (max - r_num[0] != 1)
                r_num[1] = t.Next(1, max - r_num[0] + 1);
            else
                r_num[1] = 1;

            variants[0] = r_num[0] + r_num[1];

            Gen_Variants();
            Shuffle_Variants();

            return ToString();
        }

        /*  Priority bounds of variants[] (weighting system):
            [0] Right answer
            [1-10] 1st priority options
            [11-20] 2nd priority options
            [21-30] 3rd priority options
        */

        public void Gen_Variants()
        {
            MersenneTwister t = new MersenneTwister();

            Int32 mult = r_num[0] * r_num[1];
            Int32 sum_low = (r_num[0] - 1) + (r_num[1] - 1);
            Int32 sum_high = (r_num[0] + 1) + (r_num[1] + 1);
            Int32 sum_dec_1 = r_num[0] + 10;
            Int32 sum_dec_2 = r_num[1] + 10;
            Int32 sub = r_num[0] - r_num[1];
            if (sub < 0) sub = sub * (-1);
            bool areEqual = r_num[0] == r_num[1];

            // First question generator
            if (variants[0] <= 10)
            {
                // 1ST PRIORITY

                if (variants[0] > 2)
                    variants[1] = variants[0] - t.Next(1, 3);
                else if (variants[0] == 2)
                    variants[1] = variants[0] - 1;

                if (areEqual && mult <= 9)
                    variants[2] = mult;
                
                if (variants[0] == 6)
                    variants[3] = 9;
                else if (variants[0] == 9)
                    variants[3] = 6;

                if (!areEqual && sub > 2)
                    variants[4] = sub;
                if (areEqual
                    && r_num[0] != 5
                    && r_num[1] != 5)
                    variants[5] = 10;

                // 2ND PRIORITY

                if (variants[0] < 9)
                {
                    variants[11] = variants[0] + t.Next(1, 3);
                    variants[12] = variants[0] + t.Next(1, 3);
                }

                if (areEqual && r_num[0] % 2 == 0)
                    variants[13] = r_num[0] / 2;

                if (variants[0] < 10)
                    variants[14] = variants[0] + 1;
                else if (variants[0] == 10)
                    variants[14] = variants[0] - t.Next(1, 3);

                // 3RD PRIORITY

                variants[21] = r_num[0];
                variants[22] = r_num[1];
            }

            // Second question generator
            else if (10 < variants[0] && variants[0] < 20)
            {
                // 1ST PRIORITY

                if (12 < variants[0] && variants[0] < 18)
                {
                    variants[1] = variants[0] - t.Next(1, 3);
                    variants[2] = variants[0] + t.Next(1, 3);
                }
                else
                {
                    if (variants[0] == 12 || variants[0] == 18)
                    {
                        variants[1] = variants[0] - 1;
                        variants[2] = variants[0] + 1;
                    }
                    else if (variants[0] == 11 || variants[0] == 19)
                    {
                        if (variants[0] == 11)
                        {
                            variants[1] = 12;
                            variants[2] = 13;
                        }
                        else
                        {
                            variants[1] = 18;
                            variants[2] = 17;
                        }
                    }
                }

                if (areEqual)
                {
                    if (sum_low > 10 && sum_high < 20)
                    {
                        variants[3] = sum_high;
                        variants[4] = sum_low;
                    }
                    else if (sum_low > 10)
                        variants[3] = sum_low;
                    else if (sum_high < 20)
                        variants[3] = sum_high;
                    if (variants[4] == 0)
                    {
                        if (r_num[0] == 6)
                            variants[4] = 16;
                        else if (r_num[0] == 9)
                            variants[4] = 19;
                    }
                }

                if (sum_dec_1 < 20 && sum_dec_2 < 20)
                {
                    variants[5] = sum_dec_1;
                    variants[6] = sum_dec_2;
                }
                else if (sum_dec_1 < 20)
                {
                    variants[5] = sum_dec_1;
                    variants[6] = t.Next(15, 20);
                }
                else if (sum_dec_2 < 20)
                {
                    variants[5] = sum_dec_2;
                    variants[6] = t.Next(15, 20);
                }

                // 3RD PRIORITY
                // In case there are no three different options of 1st priority

                for (int i = 21; i < 30; i++)
                    variants[i] = i - 10;
            }

            // Third question generator
            else if (20 < variants[0] && variants[0] < 100)
            {
                // 1ST PRIORITY

                if (21 < variants[0] && variants[0] < 99)
                {
                    variants[1] = variants[0] - 1;
                    variants[2] = variants[0] + 1;
                }

                if (r_num[1] % 10 != 0)
                    variants[3] = r_num[0] + (r_num[1] % 10);

                if (r_num[0] % 10 != 0)
                    variants[4] = r_num[1] + (r_num[0] % 10);

                if (variants[0] % 10 == 0 || variants[0] % 5 == 0)
                {
                    if (variants[0] + 10 < 100 && variants[0] - 10 > 20)
                    {
                        variants[5] = variants[0] + 10;
                        variants[6] = variants[0] - 10;
                        variants[7] = variants[0] + 5;
                        variants[8] = variants[0] - 5;
                    }
                    else if (variants[0] + 10 < 100 && variants[0] - 5 > 20)
                    {
                        variants[5] = variants[0] + 10;
                        variants[6] = variants[0] + 5;
                        variants[7] = variants[0] - 5;
                        if (variants[0] + 15 < 100)
                            variants[8] = variants[0] + 15;
                        else
                            variants[8] = variants[0] - 15;
                    }
                    else if (variants[0] + 5 < 100 && variants[0] - 10 > 20)
                    {
                        variants[5] = variants[0] - 10;
                        variants[6] = variants[0] + 5;
                        variants[7] = variants[0] - 5;
                        if (variants[0] - 15 > 20)
                            variants[8] = variants[0] - 15;
                        else
                            variants[8] = variants[0] + 15;
                    }
                }

                if (variants[0] % 11 == 0)
                {
                    if (variants[0] + 11 < 100 && variants[0] - 11 > 20)
                    {
                        variants[5] = variants[0] + 11;
                        variants[6] = variants[0] - 11;
                    }
                    else if (variants[0] + 11 < 100)
                    {
                        variants[5] = variants[0] + 11;
                        if (variants[0] + 22 < 100)
                            variants[6] = variants[0] + 22;
                        else
                            variants[6] = variants[0] - 22;
                    }
                    else if (variants[0] - 11 > 20)
                    {
                        variants[5] = variants[0] - 11;
                        if (variants[0] - 22 > 20)
                            variants[6] = variants[0] - 22;
                        else
                            variants[6] = variants[0] + 22;
                    }
                }

                // 2ND PRIORITY

                if (r_num[0] > r_num[1])
                {
                    variants[11] = r_num[0] + 10 + r_num[1] % 10;

                    if (r_num[1] % 2 == 0)
                        variants[12] = r_num[0] + r_num[1] / 2;
                    else if (r_num[1] % 3 == 0)
                        variants[12] = r_num[0] + r_num[1] / 3;

                    if (r_num[0] % 10 == r_num[1])
                    {
                        if (r_num[0] % 2 == 0)
                            variants[13] = variants[0] - (r_num[1] / 2);
                        else
                            variants[13] = r_num[0] - (r_num[0] % 10) + r_num[1] * r_num[1];
                    }
                }
                else if (r_num[1] > r_num[0])
                {
                    variants[11] = r_num[1] + 10 + r_num[0] % 10;

                    if (r_num[0] % 2 == 0)
                        variants[12] = r_num[1] + r_num[0] / 2;
                    else if (r_num[0] % 3 == 0)
                        variants[12] = r_num[1] + r_num[0] / 3;

                    if (r_num[1] % 10 == r_num[0])
                    {
                        if (r_num[1] % 2 == 0)
                            variants[13] = variants[0] - (r_num[0] / 2);
                        else
                            variants[13] = r_num[1] - (r_num[1] % 10) + r_num[0] * r_num[0];
                    }     
                }

                if (variants[0] % 10 != 0)
                {
                    variants[14] = variants[0] - (variants[0] % 10);
                    variants[15] = variants[0] + (variants[0] % 10);
                }

                // 3RD PRIORITY
                // In case there are no three different options of 1st and 2nd priority

                if (sub > r_num[0] - 10 || sub > r_num[1] - 10)
                    variants[21] = sub;

                for (int i = 22; i < 30; i++)
                    variants[i] = t.Next(21, 100);
            }
        }

        public void Shuffle_Variants()
        {
            Random r = new Random();

            Int32[] options = new Int32[4];

            options[0] = variants[0];

            options[1] = variants[r.Next(1, 11)];
            while (options[1] == options[0]
                || options[1] == 0
                || options[1] > max
                || options[1] < min)
                options[1] = variants[r.Next(1, 11)];

            options[2] = variants[r.Next(1, 21)];
            while (options[2] == options[0]
                || options[2] == options[1] 
                || options[2] == 0
                || options[2] > max
                || options[2] < min)
                options[2] = variants[r.Next(1, 21)];

           options[3] = variants[r.Next(1, 31)];
           while (options[3] == options[0]
                || options[3] == options[1] 
                || options[3] == options[2]
                || options[3] == 0
                || options[3] > max
                || options[3] < min)
                options[3] = variants[r.Next(1, 31)];

           // Shuffle the final options array
           // by using Fisher-Yates Shuffle algorithm

            List<Int32> shuffled = new List<Int32>();
            List<Int32> original = new List<Int32>(options);

            int i = 0;
            while (original.Count > 0)
            {
                i = r.Next(original.Count);
                shuffled.Add(original[i]);
                original.RemoveAt(i);
            }

            final_options = shuffled.ToArray();
        }

        override public String ToString()
        {
            return (r_num[0] + " + " + r_num[1] + " = ?; " 
                + final_options[0] + ", " 
                + final_options[1] + ", " 
                + final_options[2] + ", " 
                + final_options[3]);
        }
    }
}
