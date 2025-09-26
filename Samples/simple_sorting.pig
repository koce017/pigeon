void main()
{
    let a = float[];
    let n = prompt_i("n: ");

    for i = 0 to n - 1
        list_add(a, prompt_f("a[" + i + "]: "));

    for i = 0 to n - 2
    {
        for j = i to n - 1
        {
            if (a[i] > a[j])
            {
                let temp = a[i];
                a[i] = a[j];
                a[j] = temp;
            }
        }
    }

    for i = 0 to n - 1
        print(a[i]);
}