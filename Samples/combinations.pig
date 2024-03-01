let n = prompt_i("n: ");
let k = prompt_i("k: ");
let r = comb(n, k);
print(r);

int comb(int n, int k) {
    return fact(n) / (fact(k) * fact(n - k));
}

int fact(int n) {
    let f = 1;
    for i = 1 to n {
        f *= i;
    }
    return f;
}