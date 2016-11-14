using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;

namespace PendulumAnalysis
{
    class Analysis
    {   
        static void Main(string[] args)
        {
            //User Input
            Console.Out.WriteLine("Enter Red");
            int Rin = Convert.ToInt32(Console.ReadLine());
            Console.Out.WriteLine("Enter Green");
            int Gin = Convert.ToInt32(Console.ReadLine());
            Console.Out.WriteLine("Enter Blue");
            int Bin = Convert.ToInt32(Console.ReadLine());
            Console.Out.WriteLine("Enter X");
            int Xin = Convert.ToInt32(Console.ReadLine());
            Console.Out.WriteLine("Enter Y");
            int Yin = Convert.ToInt32(Console.ReadLine());
            Console.Out.WriteLine("Enter First Frame Number (File Name integer)");
            int Fileinfir = Convert.ToInt32(Console.ReadLine());
            Console.Out.WriteLine("Enter Last Frame Number (File Name integer)");
            int Fileinlas = Convert.ToInt32(Console.ReadLine());

            //Temporary File Input
            int temp = Fileinfir;

            //Loops through given frames
            for (int d = 0; d<=Fileinlas-Fileinfir; d++) {
                //Sends in image
                Bitmap img = new Bitmap(@"../pics/frame"+temp+".png");
                Console.Out.Write(temp);
                temp++;
                
                //New Array for frame size
                String[,] prethresh = new string[img.Height, img.Width];

                //Check values
                Console.Out.WriteLine(img.Height);
                Console.Out.WriteLine(img.Width);

                //Loops through each pixel getting RGB value
                for (int i = 0; i < img.Width; i++)
                {
                    for (int j = 0; j < img.Height; j++)
                    {
                        //Converts each pixel to give RGB value
                        Color pixel = img.GetPixel(i, j);
                        String a = pixel.R.ToString();
                        String b = pixel.G.ToString();
                        String c = pixel.B.ToString();

                        //Parse Int
                        int x = Int32.Parse(a);
                        int y = Int32.Parse(b);
                        int z = Int32.Parse(c);

                        //Threshold to pick up x Colour and assign as O
                        if (x == Rin && y == Gin && z == Bin)
                        {
                            prethresh[j, i] = "O";
                        }
                        else
                        {
                            prethresh[j, i] = ".";
                        }
                    }
                }

                //Assigns mid vector
                prethresh = AssignMid(prethresh, Xin, Yin);

                //Limits the centre for calculations
                prethresh = LimCen(prethresh, Yin, d);
            }
            Console.ReadLine();
        }

        //Method assigning mid vecotr
        static String[,] AssignMid(String[,] d,int xin,int yin)
        {   
                //Input co-ordinates for y value, Assigns mid vector
                for (int j = yin; j < d.GetLength(0);j++) 
                {
                //Marks mid vector as "X"
                d[j, xin] = "X";
                }
            return d;
        }

        //Calculates Components, Also Limits Centres 
        static String[,] LimCen(String[,] inp, int yin, int frame)
        {
            //Checker
            Boolean firrun = true;

            //Runs through 2D array 
            for (int i = 0;i<inp.GetLength(0);i++)
            {
                for (int j = 0; j<inp.GetLength(1);j++)
                {
                    if (inp[i, j] == "O" && firrun == true)
                    {
                        //First Detected Pixel
                        inp[i, j] = "Z";

                        //Calculates x,y components and the angle
                        int xco = CalcCompX(inp,j,i);
                        Console.Out.WriteLine(xco);
                        int yco = CalcCompY(inp,xco,j,i,yin);
                        Console.Out.WriteLine(yco);
                        double angle = CalcAngle(xco,yco);
                        angle = angle * (180 / Math.PI);
                        Console.Out.WriteLine(angle);

                        //Sends text to text file
                        String text = "angle :" + Convert.ToString(angle)+", frame: " + frame + Environment.NewLine;
                        File.WriteAllText(@"../text/Output.txt", text);
                        firrun = false;
                    }
                }
            }
            //Returns updates Bitmap
            return inp;
        }

        //Calculates angle using standard trig ratios
        static double CalcAngle(int x, int y)
        {
            int newx = (x * x);
            int newy = (y*y);
            double angle = Math.Atan2((x+1),(y+1));
            return angle;
        }

        //Calculates Y component
        static int CalcCompY(String[,]inpu,int shift, int x, int y, int ylim)
        {
            //Counter
            int ycounter = 0;
            
            //Counts up the mid vector 
            for (int a = y; a > ylim; a--)
            {
                //Adjusts so it loops under mid vector
                if (inpu[a , (x - (shift))] == "X")
                {
                    ycounter++;
                }
            }
            //Returns magnitude of Y
            return ycounter;
        }

        //Calculates X component
        static int CalcCompX(String[,]input,int x, int y)
        {
            //Initialize variable
            int counterx = 0;   

            //Checks right and counts up the pixels
            for (int a=x;a<input.GetLength(1);a++)
            {
                if (input[y,a]=="X")
                {
                    //If mid has been detected return value
                    return counterx;
                }
                else
                {
                    counterx--;
                }
            }
            counterx = 0;

            //Checks left and counts up the pixels
            for (int a = x; a > 0; a--)
            {
                if (input[y, a] == "X")
                {
                    //If mid has been detected return value
                    return counterx;
                }
                else
                {
                    counterx++;
                }
            }
            //Place holder return value
            return 3;
        }
    }
}

