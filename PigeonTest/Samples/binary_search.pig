int binary_search(list<int> a, int n, int t)
{
    let l = 0;
    let r = n - 1;

    while (l <= r)
    {
        let m = (l + r) / 2;

        if (a[m] == t)
            return m;
        else if (a[m] > t)
            r = m - 1;
        else
            l = m + 1;
    }
    
    return -1;
}

void main()
{
    let a = list<int>();
    let n = prompt_i("n: ");
    let t = prompt_i("t: ");

    for i = 0 to n - 1
        list_add(a, prompt_i("a[" + i + "]: "));

    print(binary_search(a, n, t));
}
