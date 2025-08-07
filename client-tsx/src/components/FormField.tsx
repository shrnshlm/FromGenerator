import React from 'react';
import type { FormField as FormFieldType } from '../types/api';

interface FormFieldProps {
  field: FormFieldType;
  value: string;
  onChange: (value: string) => void;
  hasError?: boolean;
}

const FormField: React.FC<FormFieldProps> = ({ field, value, onChange, hasError }) => {
  const commonClassName = `w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
    hasError ? 'border-red-300' : 'border-gray-300'
  }`;

  const renderField = () => {
    switch (field.type) {
      case 'text':
      case 'email':
      case 'tel':
      case 'date':
      case 'number':
        return (
          <input
            type={field.type}
            id={field.name}
            name={field.name}
            className={commonClassName}
            placeholder={field.placeholder}
            value={value}
            onChange={(e) => onChange(e.target.value)}
            required={field.required}
          />
        );

      case 'textarea':
        return (
          <textarea
            id={field.name}
            name={field.name}
            className={`${commonClassName} resize-none`}
            placeholder={field.placeholder}
            value={value}
            onChange={(e) => onChange(e.target.value)}
            required={field.required}
            rows={4}
          />
        );

      case 'select':
        return (
          <select
            id={field.name}
            name={field.name}
            className={commonClassName}
            value={value}
            onChange={(e) => onChange(e.target.value)}
            required={field.required}
          >
            <option value="">Please select...</option>
            {field.options.map((option, index) => (
              <option key={index} value={option}>
                {option}
              </option>
            ))}
          </select>
        );

      case 'radio':
        return (
          <div className="space-y-2">
            {field.options.map((option, index) => (
              <label key={index} className="flex items-center space-x-2 cursor-pointer">
                <input
                  type="radio"
                  name={field.name}
                  value={option}
                  checked={value === option}
                  onChange={(e) => onChange(e.target.value)}
                  className="text-blue-600"
                  required={field.required}
                />
                <span>{option}</span>
              </label>
            ))}
          </div>
        );

      case 'checkbox':
        return (
          <label className="flex items-center space-x-2 cursor-pointer">
            <input
              type="checkbox"
              name={field.name}
              checked={value === 'true'}
              onChange={(e) => onChange(e.target.checked ? 'true' : 'false')}
              className="text-blue-600"
            />
            <span>{field.label}</span>
          </label>
        );

      default:
        return (
          <input
            type="text"
            id={field.name}
            name={field.name}
            className={commonClassName}
            placeholder={field.placeholder}
            value={value}
            onChange={(e) => onChange(e.target.value)}
            required={field.required}
          />
        );
    }
  };

  return (
    <div>
      {field.type !== 'checkbox' && (
        <label
          htmlFor={field.name}
          className="block text-sm font-medium text-gray-700 mb-2"
        >
          {field.label}
          {field.required && <span className="text-red-500 ml-1">*</span>}
        </label>
      )}

      {renderField()}

      {field.required && !value && hasError && (
        <p className="text-sm text-red-600 mt-1">This field is required</p>
      )}
    </div>
  );
};

export default FormField;