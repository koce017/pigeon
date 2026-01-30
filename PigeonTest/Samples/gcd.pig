int gcd(int a, int b)
{
    let min = a < b ? a : b;
    for i = min to 1
        if (a % i == 0 && b % i == 0)
            return i;
    return 1;
}

void main()
{
    let a = prompt_i("a: ");
    let b = prompt_i("b: ");
    let r = gcd(a, b);
    print(r);
}
