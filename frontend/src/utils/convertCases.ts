/**
 * Делает первую букву строки прописной
 * @example capitalize('abcd') => 'Abcd'
 * @param {string} str - Строка для преобразования
 */
export const capitalize = (str: string) =>
  str.slice(0, 1).toUpperCase() + str.slice(1);

/**
 * Делает первую букву строки строчной
 * @example unCapitalize('ABCD') => 'aBCD'
 * @param {string} str - Строка для преобразования
 */
export const unCapitalize = (str: string) =>
  str.slice(0, 1).toLowerCase() + str.slice(1);

/**
 * Переводит строку из `snake_case` в `PascalCase`
 * @example snakeToPascal('snake_case') => 'SnakeCase'
 * @param {string} string - Строка для преобразования
 */
export const snakeToPascal = (str: string) =>
  str.split("_").map(capitalize).join("");

/**
 * Переводит строку из `snake_case` в `camelCase`
 * @example snakeToPascal('snake_case') => 'camelCase'
 * @param {string} str - Строка для преобразования
 */
export const snakeToCamel = (str: string) => unCapitalize(snakeToPascal(str));

/**
 * Переводит строку из `PascalCase` в `snake_case`
 * @example snakeToPascal('PascalCase') => 'pascal_case'
 * @param {string} str - Строка для преобразования
 */
export const pascalToSnake = (str: string) =>
  str
    .split(/(?=[A-Z])/)
    .map((str) => str.toLowerCase())
    .join("_");

export const transformObjKeysRecursive = (obj, func) => {
  if (!(obj && (Array.isArray(obj) || typeof obj === "object"))) {
    return obj;
  }

  const newObj = Array.isArray(obj) ? [] : {};

  Object.keys(obj).forEach((key) => {
    if (obj[key] == null) {
      newObj[func(key)] = obj[key];
    } else if (Array.isArray(obj[key])) {
      newObj[func(key)] = obj[key].map((value) =>
        transformObjKeysRecursive(value, func)
      );
    } else if (typeof obj[key] === "object") {
      newObj[func(key)] = transformObjKeysRecursive(obj[key], func);
    } else {
      newObj[func(key)] = obj[key];
    }
  });

  return newObj;
};

export const convertObjectToCamel = (obj: Record<string, unknown>) =>
  transformObjKeysRecursive(obj, snakeToCamel);

export const convertObjectToSnake = (obj: Record<string, unknown>) =>
  transformObjKeysRecursive(obj, pascalToSnake);

export const simpleArrayToDict = (array: string[], id = "id") => {
  if (!array || !Array.isArray(array)) {
    return array;
  }

  return array.reduce((result, item) => {
    result[item[id]] = item;
    return result;
  }, {});
};
