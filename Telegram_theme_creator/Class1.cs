using System;
using System.Drawing;

public class HSBA
{
    public double H;
    public double S;
    public double B;
    public double A;

    public void FromHSBA(double h, double s, double b, double a)
    {
        H = h;
        S = s;
        B = b;
        A = a;
    }

}