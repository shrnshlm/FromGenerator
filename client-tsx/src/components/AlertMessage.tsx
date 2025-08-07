import React from 'react';

interface AlertMessageProps {
  type: 'success' | 'error';
  message: string;
}

const AlertMessage: React.FC<AlertMessageProps> = ({ type, message }) => {
  const isSuccess = type === 'success';
  
  return (
    <div className={`rounded-lg p-4 mb-6 ${
      isSuccess 
        ? 'bg-green-50 border border-green-200' 
        : 'bg-red-50 border border-red-200'
    }`}>
      <div className="flex items-center">
        {isSuccess ? (
          <svg className="w-5 h-5 text-green-600 mr-2" fill="currentColor" viewBox="0 0 20 20">
            <path 
              fillRule="evenodd" 
              d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" 
              clipRule="evenodd" 
            />
          </svg>
        ) : (
          <svg className="w-5 h-5 text-red-600 mr-2" fill="currentColor" viewBox="0 0 20 20">
            <path 
              fillRule="evenodd" 
              d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" 
              clipRule="evenodd" 
            />
          </svg>
        )}
        <span className={`font-medium ${
          isSuccess ? 'text-green-800' : 'text-red-800'
        }`}>
          {message}
        </span>
      </div>
    </div>
  );
};

export default AlertMessage;