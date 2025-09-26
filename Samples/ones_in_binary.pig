int ones(int n)
{
    let i = 0;
    while (n > 0)
    {
        if (n % 2 == 1)
            i += 1;
        n = n / 2;
    }
    return i;
}

void main()
{
    let n = prompt_i("n: ");
    let r = ones(n);
    print(r);
}
