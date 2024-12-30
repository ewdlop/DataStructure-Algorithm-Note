def bucket_sort(arr):
    largest = max(arr)
    length = len(arr)
    size = largest / length

    # Create empty buckets
    buckets = [[] for _ in range(length)]

    # Put array elements in different buckets
    for i in range(length):
        index = int(arr[i] / size)
        if index != length:
            buckets[index].append(arr[i])
        else:
            buckets[length - 1].append(arr[i])

    # Sort individual buckets
    for i in range(length):
        buckets[i] = sorted(buckets[i])

    # Concatenate all buckets into arr
    result = []
    for i in range(length):
        result.extend(buckets[i])

    return result

# Example usage
arr = [0.42, 0.32, 0.23, 0.52, 0.25, 0.47, 0.51]
sorted_arr = bucket_sort(arr)
print(sorted_arr)