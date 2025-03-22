let s = {};
let n = prompt_i("n: ");

for i = 0 to n - 1
{
    let a = prompt_s("a: ");
    if !set_in(s, a)
    {
        set_add(s, a);
        print(a);
    }
}