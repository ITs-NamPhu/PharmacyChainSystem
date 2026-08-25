import { useState, useEffect } from 'react';

const SearchBar = ({ value, onSearch, placeholder = 'Tìm kiếm...' }) => {
    const [inputValue, setInputValue] = useState(value || '');

    useEffect(() => {
        setInputValue(value || '');
    }, [value]);

    useEffect(() => {
        const timer = setTimeout(() => {
            if (inputValue !== (value || '')) {
                onSearch(inputValue);
            }
        }, 400);
        return () => clearTimeout(timer);
    }, [inputValue]);

    return (
        <div className="filter-search-bar mb-3">
            <input
                type="text"
                className="form-control"
                placeholder={placeholder}
                value={inputValue}
                onChange={(e) => setInputValue(e.target.value)}
            />
        </div>
    );
};

export default SearchBar;
