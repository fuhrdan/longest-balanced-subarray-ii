//*****************************************************************************
//** 3721. Longest Balanced Subarray II                             leetcode **
//*****************************************************************************
//** Lazy trees whisper as balances shift,                                   **
//** Even and odd send their weighted gift,                                  **
//** Range adds ripple through prefix air,                                   **
//** Until equal echoes mark the longest pair.                               **
//*****************************************************************************
//** Top code is my own (not working currently),
//** bottom code is chatGPT's implementation (working)
//*****************************************************************************

/*
int longestBalanced(int* nums, int numsSize) {
    int coffee = 0;
    int even = 0;
    int odd = 0;
    int unique[100001] = {0};

    for(int i = 0; i < numsSize; i++)
    {
        if(nums[i] % 2 == 0)
        {
            if(unique[nums[i]] == 0)
            {
                unique[nums[i]]++;
                even++;
            }
            else
            {
                //Not Unique
            }
        }
        else
        {
            if(unique[nums[i]] == 0)
            {
                unique[nums[i]]++;
                odd++;
            }
            else
            {
                //Not Unique
            }
        }
    }
    printf("even = %d odd = %d\n",even,odd);
    if(even > odd) return odd;
    else return even;
}
*/
#define MAXN 100005

typedef struct
{
    int mn;
    int mx;
    int lazy;
} Node;

typedef struct
{
    int n;
    Node* st;
} SegTree;

/* ---------- Segment Tree Functions ---------- */

void seg_apply(SegTree* seg, int p, int val)
{
    seg->st[p].mn += val;
    seg->st[p].mx += val;
    seg->st[p].lazy += val;
}

void seg_push(SegTree* seg, int p)
{
    if (seg->st[p].lazy != 0)
    {
        seg_apply(seg, p << 1, seg->st[p].lazy);
        seg_apply(seg, p << 1 | 1, seg->st[p].lazy);
        seg->st[p].lazy = 0;
    }
}

void seg_pull(SegTree* seg, int p)
{
    int left = p << 1;
    int right = p << 1 | 1;

    seg->st[p].mn = seg->st[left].mn < seg->st[right].mn ? seg->st[left].mn : seg->st[right].mn;
    seg->st[p].mx = seg->st[left].mx > seg->st[right].mx ? seg->st[left].mx : seg->st[right].mx;
}

void seg_range_add_internal(SegTree* seg, int p, int l, int r, int L, int R, int val)
{
    if (L > R || r < L || R < l)
        return;

    if (L <= l && r <= R)
    {
        seg_apply(seg, p, val);
        return;
    }

    seg_push(seg, p);

    int m = (l + r) >> 1;

    seg_range_add_internal(seg, p << 1, l, m, L, R, val);
    seg_range_add_internal(seg, p << 1 | 1, m + 1, r, L, R, val);

    seg_pull(seg, p);
}

void seg_range_add(SegTree* seg, int L, int R, int val)
{
    if (L > R)
        return;

    seg_range_add_internal(seg, 1, 0, seg->n, L, R, val);
}

int seg_find_first_equal_internal(SegTree* seg, int p, int l, int r, int L, int R, int target)
{
    if (L > R || r < L || R < l)
        return -1;

    if (seg->st[p].mn > target || seg->st[p].mx < target)
        return -1;

    if (l == r)
        return l;

    seg_push(seg, p);

    int m = (l + r) >> 1;

    int res = seg_find_first_equal_internal(seg, p << 1, l, m, L, R, target);
    if (res != -1)
        return res;

    return seg_find_first_equal_internal(seg, p << 1 | 1, m + 1, r, L, R, target);
}

int seg_find_first_equal(SegTree* seg, int L, int R, int target)
{
    if (L > R)
        return -1;

    return seg_find_first_equal_internal(seg, 1, 0, seg->n, L, R, target);
}

int seg_point_query_internal(SegTree* seg, int p, int l, int r, int idx)
{
    if (l == r)
        return seg->st[p].mn;

    seg_push(seg, p);

    int m = (l + r) >> 1;

    if (idx <= m)
        return seg_point_query_internal(seg, p << 1, l, m, idx);
    else
        return seg_point_query_internal(seg, p << 1 | 1, m + 1, r, idx);
}

int seg_point_query(SegTree* seg, int idx)
{
    return seg_point_query_internal(seg, 1, 0, seg->n, idx);
}

void seg_init(SegTree* seg, int n)
{
    seg->n = n;
    seg->st = (Node*)calloc(4 * (n + 5), sizeof(Node));
}

/* ---------- Hash Map Replacement ---------- */
/* Since nums[i] <= 100000, we can use direct array */

int longestBalanced(int* nums, int numsSize)
{
    int n = numsSize;

    SegTree seg;
    seg_init(&seg, n);

    int* lastPos = (int*)calloc(100001, sizeof(int));

    int ret = 0;

    for (int r = 1; r <= n; r++)
    {
        int v = nums[r - 1];
        int sign = (v % 2 == 0) ? 1 : -1;

        int prev = lastPos[v];

        if (prev == 0)
        {
            seg_range_add(&seg, r, n, sign);
        }
        else
        {
            if (prev <= r - 1)
            {
                seg_range_add(&seg, prev, r - 1, -sign);
            }
        }

        lastPos[v] = r;

        int cur = seg_point_query(&seg, r);
        int idx = seg_find_first_equal(&seg, 0, r - 1, cur);

        if (idx != -1)
        {
            int len = r - idx;
            if (len > ret)
                ret = len;
        }
    }

    free(lastPos);
    free(seg.st);

    return ret;
}
