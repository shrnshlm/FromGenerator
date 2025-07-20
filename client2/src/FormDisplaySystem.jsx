import React, { useState } from 'react';

const FormDisplaySystem = () => {
    const [userInput, setUserInput] = useState('');
    const [generatedForm, setGeneratedForm] = useState(null);
    const [formData, setFormData] = useState({});
    const [isGenerating, setIsGenerating] = useState(false);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [submitResult, setSubmitResult] = useState(null);

    // Step 1: Generate form from user text
    const generateForm = async () => {
        if (!userInput.trim()) {
            alert('Please enter some text');
            return;
        }

        setIsGenerating(true);
        try {
            const response = await fetch('https://localhost:7061/api/form/generate', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    text: userInput,
                    userId: 'user123'
                }),
            });

            if (response.ok) {
                const form = await response.json();
                setGeneratedForm(form);

                // Initialize form data with pre-filled values
                const initialData = {};
                form.fields.forEach(field => {
                    initialData[field.name] = field.value || '';
                });
                setFormData(initialData);
                setSubmitResult(null);
            } else {
                const error = await response.json();
                alert(`Error: ${error.error || 'Failed to generate form'}`);
            }
        } catch (error) {
            console.error('Error generating form:', error);
            alert('Network error occurred');
        } finally {
            setIsGenerating(false);
        }
    };

    // Step 2: Handle form field changes
    const handleFieldChange = (fieldName, value) => {
        setFormData(prev => ({
            ...prev,
            [fieldName]: value
        }));
    };

    // Step 3: Submit the completed form
    const submitForm = async (e) => {
        e.preventDefault();

        // Validate required fields
        const requiredFields = generatedForm.fields.filter(field => field.required);
        const missingFields = requiredFields.filter(field => !formData[field.name]?.trim());

        if (missingFields.length > 0) {
            alert(`Please fill in required fields: ${missingFields.map(f => f.label).join(', ')}`);
            return;
        }

        setIsSubmitting(true);
        try {
            const response = await fetch('/api/form/submit', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    formId: generatedForm.formId,
                    fieldValues: formData
                }),
            });

            const result = await response.json();

            if (response.ok) {
                setSubmitResult({ success: true, message: result.message });
                // Optionally reset form after successful submission
                setTimeout(() => {
                    setGeneratedForm(null);
                    setFormData({});
                    setUserInput('');
                    setSubmitResult(null);
                }, 3000);
            } else {
                setSubmitResult({ success: false, message: result.error || 'Submission failed' });
            }
        } catch (error) {
            console.error('Error submitting form:', error);
            setSubmitResult({ success: false, message: 'Network error occurred' });
        } finally {
            setIsSubmitting(false);
        }
    };

    // Step 4: Render different field types
    const renderField = (field) => {
        const commonProps = {
            id: field.name,
            name: field.name,
            className: `w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                field.required && !formData[field.name] ? 'border-red-300' : 'border-gray-300'
            }`,
            placeholder: field.placeholder,
            value: formData[field.name] || '',
            onChange: (e) => handleFieldChange(field.name, e.target.value),
            required: field.required
        };

        switch (field.type) {
            case 'text':
            case 'email':
            case 'tel':
            case 'date':
            case 'number':
                return <input {...commonProps} type={field.type} />;

            case 'textarea':
                return (
                    <textarea
                        {...commonProps}
                        rows={4}
                        className={`${commonProps.className} resize-none`}
                    />
                );

            case 'select':
                return (
                    <select {...commonProps}>
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
                                    checked={formData[field.name] === option}
                                    onChange={(e) => handleFieldChange(field.name, e.target.value)}
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
                            checked={formData[field.name] === 'true'}
                            onChange={(e) => handleFieldChange(field.name, e.target.checked ? 'true' : 'false')}
                            className="text-blue-600"
                        />
                        <span>{field.label}</span>
                    </label>
                );

            default:
                return <input {...commonProps} type="text" />;
        }
    };

    return (
        <div className="max-w-4xl mx-auto p-6 bg-gray-50 min-h-screen">
            <h1 className="text-3xl font-bold text-center mb-8 text-gray-900">
                AI Form Generator
            </h1>

            {/* Step 1: Text Input for Form Generation */}
            <div className="bg-white rounded-lg shadow-md p-6 mb-6">
                <h2 className="text-xl font-semibold mb-4 text-gray-800">
                    What do you need help with?
                </h2>

                <div className="space-y-4">
          <textarea
              value={userInput}
              onChange={(e) => setUserInput(e.target.value)}
              placeholder="Describe what you need in natural language...&#10;&#10;Examples:&#10;• 'I want to book a flight to Tokyo'&#10;• 'I need to schedule a doctor appointment'&#10;• 'I want to register for your newsletter'&#10;• 'I have feedback about your service'"
              className="w-full px-4 py-3 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none"
              rows={4}
          />

                    <button
                        onClick={generateForm}
                        disabled={!userInput.trim() || isGenerating}
                        className="bg-blue-600 hover:bg-blue-700 disabled:bg-gray-400 text-white font-medium py-3 px-6 rounded-md transition-colors duration-200"
                    >
                        {isGenerating ? (
                            <span className="flex items-center">
                <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Generating Form...
              </span>
                        ) : (
                            'Generate Form'
                        )}
                    </button>
                </div>
            </div>

            {/* Step 2: Display Generated Form */}
            {generatedForm && (
                <div className="bg-white rounded-lg shadow-md p-6 mb-6">
                    <div className="mb-6">
                        <h2 className="text-2xl font-bold text-gray-900 mb-2">
                            {generatedForm.title}
                        </h2>
                        <div className="flex items-center space-x-4 text-sm text-gray-600">
                            <span>Detected Intent: <strong>{generatedForm.intent}</strong></span>
                            <span>Form ID: <code className="bg-gray-100 px-2 py-1 rounded">{generatedForm.formId}</code></span>
                        </div>
                    </div>

                    <form onSubmit={submitForm} className="space-y-6">
                        {generatedForm.fields.map((field, index) => (
                            <div key={index}>
                                <label
                                    htmlFor={field.name}
                                    className="block text-sm font-medium text-gray-700 mb-2"
                                >
                                    {field.label}
                                    {field.required && <span className="text-red-500 ml-1">*</span>}
                                </label>

                                {renderField(field)}

                                {field.required && !formData[field.name] && (
                                    <p className="text-sm text-red-600 mt-1">This field is required</p>
                                )}
                            </div>
                        ))}

                        <div className="flex justify-between items-center pt-6 border-t">
                            <button
                                type="button"
                                onClick={() => {
                                    setGeneratedForm(null);
                                    setFormData({});
                                    setSubmitResult(null);
                                }}
                                className="px-4 py-2 text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50 transition-colors duration-200"
                            >
                                Cancel
                            </button>

                            <button
                                type="submit"
                                disabled={isSubmitting}
                                className="bg-green-600 hover:bg-green-700 disabled:bg-gray-400 text-white font-medium py-2 px-6 rounded-md transition-colors duration-200"
                            >
                                {isSubmitting ? (
                                    <span className="flex items-center">
                    <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                    </svg>
                    Submitting...
                  </span>
                                ) : (
                                    generatedForm.submitButtonText
                                )}
                            </button>
                        </div>
                    </form>
                </div>
            )}

            {/* Step 3: Show Submission Result */}
            {submitResult && (
                <div className={`rounded-lg p-4 mb-6 ${
                    submitResult.success ? 'bg-green-50 border border-green-200' : 'bg-red-50 border border-red-200'
                }`}>
                    <div className="flex items-center">
                        {submitResult.success ? (
                            <svg className="w-5 h-5 text-green-600 mr-2" fill="currentColor" viewBox="0 0 20 20">
                                <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
                            </svg>
                        ) : (
                            <svg className="w-5 h-5 text-red-600 mr-2" fill="currentColor" viewBox="0 0 20 20">
                                <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
                            </svg>
                        )}
                        <span className={`font-medium ${
                            submitResult.success ? 'text-green-800' : 'text-red-800'
                        }`}>
              {submitResult.message}
            </span>
                    </div>
                </div>
            )}

            {/* Example Prompts */}
            {!generatedForm && (
                <div className="bg-white rounded-lg shadow-md p-6">
                    <h3 className="text-lg font-semibold mb-4 text-gray-900">Try these examples:</h3>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        {[
                            "I want to book a flight to New York",
                            "I need a hotel room in Paris for 2 nights",
                            "I have a question about your service",
                            "I want to register for your newsletter",
                            "I'd like to leave feedback about my experience",
                            "I need to schedule an appointment"
                        ].map((example, index) => (
                            <button
                                key={index}
                                onClick={() => setUserInput(example)}
                                className="text-left p-3 bg-blue-50 hover:bg-blue-100 border border-blue-200 rounded-md transition-colors duration-200"
                            >
                                <span className="text-blue-800 font-medium">"{example}"</span>
                            </button>
                        ))}
                    </div>
                </div>
            )}
        </div>
    );
};

export default FormDisplaySystem;