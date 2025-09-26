int factorial(int n)
{
    if (n == 1)
        return 1;
    return n * factorial(n - 1);
}

void main()
{
    let n = prompt_i("n: ");
    let r = factorial(n);
    print(r);
}
