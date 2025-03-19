/**
 * Делает первую букву строки прописной
 * @example capitalize('abcd') => 'Abcd'
 * @param {str} str - Строка для преобразования
 */
export const capitalize = (str: string) =>
  str.substr(0, 1).toUpperCase() + str.substr(1);

/**
 * Делает первую букву строки строчной
 * @example unCapitalize('ABCD') => 'aBCD'
 * @param {str} str - Строка для преобразования
 */
export const unCapitalize = (str: string) =>
  str.substr(0, 1).toLowerCase() + str.substr(1);

/**
 * Переводит строку из `snake_case` в `PascalCase`
 * @example snakeToPascal('snake_case') => 'SnakeCase'
 * @param {str} str - Строка для преобразования
 */
export const snakeToPascal = (str: string) =>
  str.split("_").map(capitalize).join("");

/**
 * Переводит строку из `snake_case` в `camelCase`
 * @example snakeToPascal('snake_case') => 'camelCase'
 * @param {str} str - Строка для преобразования
 */
export const snakeToCamel = (str: string) => unCapitalize(snakeToPascal(str));

/**
 * Переводит строку из `PascalCase` в `snake_case`
 * @example snakeToPascal('PascalCase') => 'pascal_case'
 * @param {str} str - Строка для преобразования
 */
export const pascalToSnake = (str: string) =>
  str
    .split(/(?=[A-Z])/)
    .map((str: string) => str.toLowerCase())
    .join("_");

export function transformObjKeysRecursively(
  obj: unknown,
  func: (key: string) => string
): unknown {
  // Если obj не объект или равен null, возвращаем как есть
  if (obj === null || typeof obj !== "object") {
    return obj;
  }

  // Если obj является массивом, обрабатываем каждый элемент
  if (Array.isArray(obj)) {
    return obj.map((item) => transformObjKeysRecursively(item, func));
  }

  // Если obj является объектом, создаём новый объект и трансформируем его ключи
  const result: Record<string, unknown> = {};
  for (const key in obj as Record<string, unknown>) {
    const value = (obj as Record<string, unknown>)[key];
    result[func(key)] = transformObjKeysRecursively(value, func);
  }

  return result;
}

export const convertObjectToCamel = (obj: unknown) =>
  transformObjKeysRecursively(obj, snakeToCamel);

export const convertObjectToSnake = (obj: unknown) =>
  transformObjKeysRecursively(obj, pascalToSnake);

export const simpleArrayToDict = (array: unknown, id = "id") => {
  if (!array || !Array.isArray(array)) {
    return array;
  }

  return array.reduce((result, item) => {
    result[item[id]] = item;
    return result;
  }, {});
};
