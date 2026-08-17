export function distinct(array) {
    let newArray = [];
    for (let i = 0; i < array.length; i++) {
        let value = array[i];

        let equals = false;
        for (let j = 0; j < newArray.length; j++) {

            if (typeof value === 'object') {
                let keys = Object.keys(value);
                equals = ArrayEquals(keys.map(k => value[k]), keys.map(k => newArray[j][k]));
            }
            else {
                equals = newArray.includes(value);
            }
            if (equals) {
                break;
            }
        }

        if (!equals) {
            newArray.push(value);
        }
    }

    return newArray;
}

export function SelectMany(arrayOfArrays, selectorFunc) {
    let newArray = [];

    arrayOfArrays.forEach(element => {
        newArray = newArray.concat(selectorFunc(element));
    });

    return newArray;
}

export function ArrayEquals(array1, array2) {
    if (array1.length !== array2.length) {
        return false;
    }

    let matched = array1.filter(x => array2.includes(x));

    return matched.length === array1.length;
}

export function DistinctBy(array, func) {
    let newArray = [];
    let mappedArray = [];
    for (let i = 0; i < array.length; i++) {
        let value = array[i];

        let equals = false;
        for (let j = 0; j < newArray.length; j++) {
            equals = mappedArray.includes(func(value));
        }

        if (!equals) {
            newArray.push(value);
            mappedArray.push(func(value));
        }
    }

    return newArray;
}

export function RemoveDuplicateAllocations(obj) {
    return Object.keys(obj).reduce((previous, current) => {
        if (Array.isArray(obj[current]) && obj[current].every(x => typeof x === 'object')) {
            previous[current] = DistinctBy(obj[current], x => x.id);
        }
        else {
            previous[current] = obj[current];
        }

        return previous;
    }, {});
}

export function sleep(delay = 0) {
    return new Promise((resolve) => {
        setTimeout(resolve, delay);
    });
}

export function objectCompare(obj1, obj2) {
    if(!ArrayEquals(Object.keys(obj1), Object.keys(obj2))) {
        return false;
    }    

    return Object.keys(obj1).reduce((prev, current, array) => {

        if(obj1[current] !== obj2[current]) {
            return false;
        }
        return prev;
    }, true);
}

export function GroupBy(array, groupFunc, selectFunc) {
    let dictionary = new Map();

    array.forEach((element, i) => {
        let grouping = groupFunc(element, i);

        if(!dictionary.has(grouping)) {
            dictionary.set(grouping, []);
        }

        let value = selectFunc ? selectFunc(element, i) : element;

        let group = dictionary.get(grouping);
        group.push(value);
    });

    return dictionary;
}