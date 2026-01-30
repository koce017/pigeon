void main()
{
    let a = list<int>();
    let b = list<int>();
    let n = prompt_i("n: ");

    for i = 0 to n - 1
    {
        let x = prompt_i("elem #" + i + ": ");
        list_add(a, x);
        list_add(b, x);
    }

    sort(a, "asc");
    sort(b, "desc");

    for i = 0 to n - 1
        print(a[i]);
    
    for i = 0 to n - 1
        print(b[i]);
}