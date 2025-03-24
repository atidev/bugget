

const SearchResults = () => {
  return (
    <div className="border border-base-300 rounded-lg p-4 hover:bg-base-100 cursor-pointer">
      <div className="flex justify-between">
        <div className="font-bold">#123 Краткое описание</div>
        <span className="badge badge-success">Решен</span>
      </div>
      <div className="text-sm text-base-content/70">Создан: 5 минут назад • Вася Пупкин</div>
    </div>
  );
};

export default SearchResults;
