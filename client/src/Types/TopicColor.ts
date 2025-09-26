export enum TopicColor {
  None = 0,
  Red = 1,
  Blue = 2,
  Green = 3,
  Yellow = 4,
  Orange = 5,
  Purple = 6,
  Pink = 7,
  Teal = 8,
  Indigo = 9,
  Gray = 10,
  Brown = 11
}

export const TopicColorOptions = [
  { value: TopicColor.None, label: 'None', hex: '#6b7280', bgClass: 'bg-gray-500' },
  { value: TopicColor.Red, label: 'Red', hex: '#ef4444', bgClass: 'bg-red-500' },
  { value: TopicColor.Blue, label: 'Blue', hex: '#3b82f6', bgClass: 'bg-blue-500' },
  { value: TopicColor.Green, label: 'Green', hex: '#10b981', bgClass: 'bg-green-500' },
  { value: TopicColor.Yellow, label: 'Yellow', hex: '#eab308', bgClass: 'bg-yellow-500' },
  { value: TopicColor.Orange, label: 'Orange', hex: '#f59e0b', bgClass: 'bg-orange-500' },
  { value: TopicColor.Purple, label: 'Purple', hex: '#8b5cf6', bgClass: 'bg-purple-500' },
  { value: TopicColor.Pink, label: 'Pink', hex: '#ec4899', bgClass: 'bg-pink-500' },
  { value: TopicColor.Teal, label: 'Teal', hex: '#06b6d4', bgClass: 'bg-teal-500' },
  { value: TopicColor.Indigo, label: 'Indigo', hex: '#6366f1', bgClass: 'bg-indigo-500' },
  { value: TopicColor.Gray, label: 'Gray', hex: '#6b7280', bgClass: 'bg-gray-500' },
  { value: TopicColor.Brown, label: 'Brown', hex: '#a3a3a3', bgClass: 'bg-amber-600' }
];
