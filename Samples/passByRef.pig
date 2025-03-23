let a = -1;
let b = {};
let c = string[];

populate(a, b, c);

print(a);

for i = 0 to 10
    if set_in(b, i)
        print(i);

for i = 0 to len(c) - 1
    print(c[i]);

void populate(int var1, set var2, []string var3)
{
    var1 = 100;
        
    for i = 1 to 10
        if i % 2 == 0
            set_add(var2, i);
    
    list_add(var3, "James");
    list_add(var3, "Bond");
    list_add(var3, "007");
}