import React, { useState } from 'react';
import { apiService, ApiError } from '../services/api';
import type { GeneratedForm } from '../types/api';
import FormField from './FormField';
import LoadingSpinner from './LoadingSpinner';
import AlertMessage from './AlertMessage';

interface FormData {
  [key: string]: string;
}

const FormGeneratorApp: React.FC = () => {
  const [userInput, setUserInput] = useState('');
  const [generatedForm, setGeneratedForm] = useState<GeneratedForm | null>(null);
  const [formData, setFormData] = useState<FormData>({});
  const [isGenerating, setIsGenerating] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [submitResult, setSubmitResult] = useState<{ type: 'success' | 'error'; message: string } | null>(null);
  const [validationErrors, setValidationErrors] = useState<Set<string>>(new Set());

  const examplePrompts = [
    "I want to book a flight to New York",
    "I need a hotel room in Paris for 2 nights",
    "I have a question about your service",
    "I want to register for your newsletter",
    "I'd like to leave feedback about my experience",
    "I need to schedule an appointment"
  ];

  const generateForm = async () => {
    if (!userInput.trim()) {
      alert('Please enter some text');
      return;
    }

    setIsGenerating(true);
    setSubmitResult(null);
    setValidationErrors(new Set());

    try {
      const form = await apiService.generateForm({
        text: userInput,
        userId: 'user123'
      });

      setGeneratedForm(form);

      const initialData: FormData = {};
      form.fields.forEach(field => {
        initialData[field.name] = field.value || '';
      });
      setFormData(initialData);
    } catch (error) {
      if (error instanceof ApiError) {
        alert(`Error: ${error.error.error || 'Failed to generate form'}`);
      } else {
        alert('Network error occurred');
      }
      console.error('Error generating form:', error);
    } finally {
      setIsGenerating(false);
    }
  };

  const handleFieldChange = (fieldName: string, value: string) => {
    setFormData(prev => ({
      ...prev,
      [fieldName]: value
    }));

    if (validationErrors.has(fieldName) && value.trim()) {
      setValidationErrors(prev => {
        const newErrors = new Set(prev);
        newErrors.delete(fieldName);
        return newErrors;
      });
    }
  };

  const validateForm = (): boolean => {
    if (!generatedForm) return false;

    const errors = new Set<string>();
    const requiredFields = generatedForm.fields.filter(field => field.required);

    requiredFields.forEach(field => {
      if (!formData[field.name]?.trim()) {
        errors.add(field.name);
      }
    });

    setValidationErrors(errors);
    return errors.size === 0;
  };

  const submitForm = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!generatedForm || !validateForm()) {
      return;
    }

    setIsSubmitting(true);

    try {
      const response = await apiService.submitForm({
        formId: generatedForm.formId,
        fieldValues: formData
      });

      setSubmitResult({
        type: 'success',
        message: response.message
      });

      setTimeout(() => {
        setGeneratedForm(null);
        setFormData({});
        setUserInput('');
        setSubmitResult(null);
        setValidationErrors(new Set());
      }, 3000);
    } catch (error) {
      if (error instanceof ApiError) {
        setSubmitResult({
          type: 'error',
          message: error.error.error || 'Submission failed'
        });
      } else {
        setSubmitResult({
          type: 'error',
          message: 'Network error occurred'
        });
      }
      console.error('Error submitting form:', error);
    } finally {
      setIsSubmitting(false);
    }
  };

  const resetForm = () => {
    setGeneratedForm(null);
    setFormData({});
    setSubmitResult(null);
    setValidationErrors(new Set());
  };

  return (
    <div className="max-w-4xl mx-auto p-6 bg-gray-50 min-h-screen">
      <h1 className="text-3xl font-bold text-center mb-8 text-gray-900">
        AI Form Generator
      </h1>

      {/* Text Input for Form Generation */}
      <div className="bg-white rounded-lg shadow-md p-6 mb-6">
        <h2 className="text-xl font-semibold mb-4 text-gray-800">
          What do you need help with?
        </h2>

        <div className="space-y-4">
          <textarea
            value={userInput}
            onChange={(e) => setUserInput(e.target.value)}
            placeholder="Describe what you need in natural language...

Examples:
• 'I want to book a flight to Tokyo'
• 'I need to schedule a doctor appointment'
• 'I want to register for your newsletter'
• 'I have feedback about your service'"
            className="w-full px-4 py-3 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none"
            rows={4}
          />

          <button
            onClick={generateForm}
            disabled={!userInput.trim() || isGenerating}
            className="bg-blue-600 hover:bg-blue-700 disabled:bg-gray-400 text-white font-medium py-3 px-6 rounded-md transition-colors duration-200 flex items-center"
          >
            {isGenerating ? (
              <>
                <LoadingSpinner />
                Generating Form...
              </>
            ) : (
              'Generate Form'
            )}
          </button>
        </div>
      </div>

      {/* Display Generated Form */}
      {generatedForm && (
        <div className="bg-white rounded-lg shadow-md p-6 mb-6">
          <div className="mb-6">
            <h2 className="text-2xl font-bold text-gray-900 mb-2">
              {generatedForm.title}
            </h2>
            <div className="flex items-center space-x-4 text-sm text-gray-600">
              <span>
                Detected Intent: <strong>{generatedForm.intent}</strong>
              </span>
              <span>
                Form ID: <code className="bg-gray-100 px-2 py-1 rounded">{generatedForm.formId}</code>
              </span>
            </div>
          </div>

          <form onSubmit={submitForm} className="space-y-6">
            {generatedForm.fields.map((field, index) => (
              <FormField
                key={index}
                field={field}
                value={formData[field.name] || ''}
                onChange={(value) => handleFieldChange(field.name, value)}
                hasError={validationErrors.has(field.name)}
              />
            ))}

            <div className="flex justify-between items-center pt-6 border-t">
              <button
                type="button"
                onClick={resetForm}
                className="px-4 py-2 text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50 transition-colors duration-200"
              >
                Cancel
              </button>

              <button
                type="submit"
                disabled={isSubmitting}
                className="bg-green-600 hover:bg-green-700 disabled:bg-gray-400 text-white font-medium py-2 px-6 rounded-md transition-colors duration-200 flex items-center"
              >
                {isSubmitting ? (
                  <>
                    <LoadingSpinner />
                    Submitting...
                  </>
                ) : (
                  generatedForm.submitButtonText
                )}
              </button>
            </div>
          </form>
        </div>
      )}

      {/* Show Submission Result */}
      {submitResult && (
        <AlertMessage type={submitResult.type} message={submitResult.message} />
      )}

      {/* Example Prompts */}
      {!generatedForm && (
        <div className="bg-white rounded-lg shadow-md p-6">
          <h3 className="text-lg font-semibold mb-4 text-gray-900">Try these examples:</h3>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {examplePrompts.map((example, index) => (
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

export default FormGeneratorApp;