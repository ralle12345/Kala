﻿using Xamarin.Forms;

public class ItemLabel : Label
{
    public static readonly BindableProperty LinkProperty = BindableProperty.Create(nameof(Link), typeof(string), typeof(ItemLabel), null);
    public static readonly BindableProperty PreProperty = BindableProperty.Create(nameof(Pre), typeof(string), typeof(ItemLabel), null);
    public static readonly BindableProperty PostProperty = BindableProperty.Create(nameof(Post), typeof(string), typeof(ItemLabel), null);
    public static readonly BindableProperty TypeProperty = BindableProperty.Create(nameof(Type), typeof(Kala.Models.Itemtypes), typeof(ItemLabel), Kala.Models.Itemtypes.Notused);
    public static readonly BindableProperty DigitsProperty = BindableProperty.Create(nameof(Digits), typeof(int), typeof(ItemLabel), -1);

    public string Link
    {
        get { return (string)GetValue(LinkProperty); }
        set { SetValue(LinkProperty, value); }
    }

    public string Pre
    {
        get { return (string)GetValue(PreProperty); }
        set { SetValue(PreProperty, value); }
    }

    public string Post
    {
        get { return (string)GetValue(PostProperty); }
        set { SetValue(PostProperty, value); }
    }

    public Kala.Models.Itemtypes Type
    {
        get { return (Kala.Models.Itemtypes)GetValue(TypeProperty); }
        set { SetValue(TypeProperty, value); }
    }

    public int Digits
    {
        get { return (int)GetValue(DigitsProperty); }
        set { SetValue(DigitsProperty, value); }
    }

}
