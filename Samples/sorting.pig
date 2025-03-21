let a = float[];
let size = prompt_i("size: ");

for i = 0 to size - 1
    list_add_f(a, prompt_f("a[" + i + "]: "));

for i = 0 to size - 2
{
    for j = i to size - 1
    {
        if (a[i] > a[j])
        {
            let temp = a[i];
            a[i] = a[j];
            a[j] = temp;
        }
    }
}

for i = 0 to size - 1
    print(a[i]);